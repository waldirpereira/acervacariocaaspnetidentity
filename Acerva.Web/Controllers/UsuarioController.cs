﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroUsuarios;
using FluentValidation;
using log4net;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    public class UsuarioController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IValidator<Usuario> _validator;
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly ICadastroRegionais _cadastroRegionais;
        private readonly UsuarioControllerHelper _helper;

        public UsuarioController(IValidator<Usuario> validator,
            ICadastroUsuarios cadastroUsuarios, ICadastroRegionais cadastroRegionais, UsuarioControllerHelper helper) : base(cadastroUsuarios)
        {
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _cadastroRegionais = cadastroRegionais;
            _helper = helper;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult BuscaParaListagem()
        {
            var listaUsuariosJson = _cadastroUsuarios.BuscaParaListagem().ToList();

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            
            // é delegado (só visualiza associados de sua regional)
            if (!usuarioLogadoEhAdmin && !usuarioLogadoEhDiretor)
            {
                if (usuarioLogadoBd == null)
                    usuarioLogadoBd = new Usuario();
                listaUsuariosJson = listaUsuariosJson.Where(u => u.Regional.Equals(usuarioLogadoBd.Regional)).ToList();
            }

            var listaUsuariosViewModel = listaUsuariosJson.Select(Mapper.Map<UsuarioViewModel>);
            return new JsonNetResult(listaUsuariosViewModel);
        }

        public ActionResult BuscaTiposDominio()
        {
            var regionaisJson = _cadastroRegionais.BuscaTodos()
                .Select(Mapper.Map<RegionalViewModel>);

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            var usuarioLogadoEhDelegado = usuarioLogado.IsInRole("DELEGADO");
            var regionalDoUsuarioLogadoJson = Mapper.Map<RegionalViewModel>(usuarioLogadoBd != null ? usuarioLogadoBd.Regional : null);

            var papeisJson = _cadastroUsuarios.BuscaTodosPapeis()
                .Select(Mapper.Map<PapelViewModel>);

            return new JsonNetResult(new
            {
                Regionais = regionaisJson,
                IdUsuarioLogado = usuarioLogado.Identity != null ? usuarioLogado.Identity.GetUserId() : null,
                RegionalDoUsuarioLogado = regionalDoUsuarioLogadoJson,
                UsuarioLogadoEhAdmin = usuarioLogadoEhAdmin,
                UsuarioLogadoEhDiretor = usuarioLogadoEhDiretor,
                UsuarioLogadoEhDelegado = usuarioLogadoEhDelegado,
                Papeis = papeisJson
            });
        }

        public ActionResult Busca(string id)
        {
            var user = _cadastroUsuarios.Busca(id);
            var userJson = Mapper.Map<UsuarioViewModel>(user);

            userJson.FotoBase64 = _helper.BuscaFotoBase64(id, HttpContext);

            return new JsonNetResult(userJson);
        }

        public ActionResult BuscaUsuarioLogadoParaEdicao()
        {
            var usuarioLogado = HttpContext.User;
            if (!usuarioLogado.Identity.IsAuthenticated)
                return null;

            return Busca(usuarioLogado.Identity.GetUserId());
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult Salva([JsonBinder]UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está salvando o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var ehNovo = string.IsNullOrEmpty(usuarioViewModel.Id);
            var usuario = ehNovo ? new Usuario() : _cadastroUsuarios.Busca(usuarioViewModel.Id);

            usuarioViewModel.Name = usuarioViewModel.Name.Trim();

            Mapper.Map(usuarioViewModel, usuario);

            if (ehNovo)
            {
                usuario.Id = Guid.NewGuid().ToString();
                usuario.CreationDate = DateTime.Now;
            }

            var validacao = _validator.Validate(usuario);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (_cadastroUsuarios.ExisteComMesmoNome(usuario))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um associado com o nome {0:unsafe}", usuario.Name));

            if (_cadastroUsuarios.ExisteComMesmoCpf(usuario))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um associado com o CPF {0:unsafe}", usuario.Cpf));

            if (ehNovo)
                _cadastroUsuarios.SalvaNovo(usuario);

            _helper.SalvaFoto(usuario.Id, usuarioViewModel.FotoBase64, HttpContext);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Usuário <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), usuario.Id, usuario.Name),
                "Usuário salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult ConfirmaEmail([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está confirmando o e-mail do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            return RedirectToAction("ConfirmEmail", "Account", new
            {
                userId = usuarioViewModel.Id,
                isJsonReturn = true
            });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult ConfirmaPagamento([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está confirmando pagamento de anuidade do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);
            usuario.Status = StatusUsuario.Ativo;
            usuario.Matricula = string.IsNullOrEmpty(usuario.Matricula) ? _cadastroUsuarios.PegaProximaMatricula() : usuario.Matricula;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado teve seu pagamento confirmado com sucesso", "Pagamento confirmado");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult ConfirmaPagamentoSelecionados([JsonBinder] IEnumerable<string> idsUsuarios)
        {
            var listaIdsUsuarios = idsUsuarios.ToList();
            Log.InfoFormat("Usuário está confirmando pagamento de anuidade dos associados de códigos {0}",
                listaIdsUsuarios.Aggregate((x, y)=> x + ", " + y));

            foreach (var userId in listaIdsUsuarios)
            {
                var usuario = _cadastroUsuarios.Busca(userId);
                usuario.Status = StatusUsuario.Ativo;
                usuario.Matricula = string.IsNullOrEmpty(usuario.Matricula) ? _cadastroUsuarios.PegaProximaMatricula() : usuario.Matricula;
            }
            
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associados tiveram seus pagamentos confirmados com sucesso", "Pagamentos confirmados");
            return new JsonNetResult(new { growlMessage });
        }
        
        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult CobrancaGerada([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está confirmando que a cobrança foi gerada para o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);
            usuario.Status = StatusUsuario.AguardandoPagamentoAnuidade;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Pretendente teve sua cobrança confirmada como gerada com sucesso", "Cobrança gerada confirmada");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult CobrancaGeradaSelecionados([JsonBinder] IEnumerable<string> idsUsuarios)
        {
            var listaIdsUsuarios = idsUsuarios.ToList();
            Log.InfoFormat("Usuário está confirmando que as cobranças foram geradas para os associados de códigos {0}",
                listaIdsUsuarios.Aggregate((x, y) => x + ", " + y));

            foreach (var userId in listaIdsUsuarios)
            {
                var usuario = _cadastroUsuarios.Busca(userId);
                usuario.Status = StatusUsuario.AguardandoPagamentoAnuidade;
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Pretendentes tiveram suas cobranças confirmadas como geradas com sucesso", "Cobranças geradas confirmadas");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult CancelaUsuario([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está cancelando o registro do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);
            usuario.Status = StatusUsuario.Cancelado;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado cancelado com sucesso", "Cancelamento confirmado");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult ReativaUsuario([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está reativando o registro do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);
            usuario.Status = StatusUsuario.Novo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado reativado com sucesso", "Reativação confirmada");
            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Usuário não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        public ActionResult BuscaUsuariosAtivosComTermo(string termo)
        {
            var usuariosDisponiveis = _cadastroUsuarios.BuscaComTermo(termo)
                .Where(u => u.Status == StatusUsuario.Ativo)
                .OrderBy(p => p.Name)
                .Take(20)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    NomeRegional = u.Regional.Nome
                });

            return new JsonNetResult(usuariosDisponiveis);
        }
    }
}