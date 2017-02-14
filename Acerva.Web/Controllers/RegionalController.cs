using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroRegionais;
using FluentValidation;
using log4net;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
    public class RegionalController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroRegionais _cadastroRegionais;
        private readonly IValidator<Regional> _validator;
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly IIdentity _user;
        
        public RegionalController(ICadastroRegionais cadastroRegionais, IValidator<Regional> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroRegionais = cadastroRegionais;
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            var usuarioLogadoEhDelegado = usuarioLogado.IsInRole("DELEGADO");

            var listaRegionaisJson = _cadastroRegionais.BuscaParaListagem()
                .Where(r => usuarioLogadoEhAdmin || usuarioLogadoEhDiretor || (usuarioLogadoEhDelegado && usuarioLogadoBd != null && usuarioLogadoBd.Regional.Equals(r)))
                .Select(Mapper.Map<RegionalViewModel>);
            return new JsonNetResult(listaRegionaisJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            var usuarioLogado = HttpContext.User;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");

            return new JsonNetResult(new
            {
                UsuarioLogadoEhAdmin = usuarioLogadoEhAdmin,
                UsuarioLogadoEhDiretor = usuarioLogadoEhDiretor
            });
        }

        public ActionResult Busca(int codigo)
        {
            var regional = _cadastroRegionais.Busca(codigo);

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            var usuarioLogadoEhDelegado = usuarioLogado.IsInRole("DELEGADO");

            if (usuarioLogadoBd == null)
                return RetornaJsonDeAlerta("Usuário logado não encontrado no BD");

            if (!usuarioLogadoEhAdmin && !usuarioLogadoEhDiretor && (!usuarioLogadoEhDelegado || !usuarioLogadoBd.Regional.Equals(regional)))
                return RetornaJsonDeAlerta("Usuário logado não é diretor ou delegado da regional buscada.");

            var regionalJson = Mapper.Map<RegionalViewModel>(regional);
            return new JsonNetResult(regionalJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]RegionalViewModel regionalViewModel)
        {
            Log.InfoFormat("Usuário está salvando a regional {0} de código {1}", regionalViewModel.Nome, regionalViewModel.Codigo);

            var ehNovo = regionalViewModel.Codigo == 0;
            var regional = ehNovo ? new Regional() : _cadastroRegionais.Busca(regionalViewModel.Codigo);

            regionalViewModel.Nome = regionalViewModel.Nome.Trim();

            Mapper.Map(regionalViewModel, regional);

            var validacao = _validator.Validate(regional);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(regional))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma regional com o nome {0:unsafe}", regional.Nome));

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            var usuarioLogadoEhDelegado = usuarioLogado.IsInRole("DELEGADO");

            if (usuarioLogadoBd == null)
                return RetornaJsonDeAlerta("Usuário logado não encontrado no BD");

            if (!usuarioLogadoEhAdmin && !usuarioLogadoEhDiretor && (!usuarioLogadoEhDelegado || !usuarioLogadoBd.Regional.Equals(regional)))
                return RetornaJsonDeAlerta("Usuário logado não é diretor ou delegado da regional em edição.");

            _cadastroRegionais.Salva(regional);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Regional <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), regional.Codigo, regional.Nome),
                "Regional salva");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AnexaLogotipo(int codigoRegional)
        {
            if (Request.Files == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var regional = _cadastroRegionais.Busca(codigoRegional);
            if (regional == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Regional não encontrada"));

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");
            var usuarioLogadoEhDelegado = usuarioLogado.IsInRole("DELEGADO");

            if (usuarioLogadoBd == null)
                return RetornaJsonDeAlerta("Usuário logado não encontrado no BD");

            if (!usuarioLogadoEhAdmin && !usuarioLogadoEhDiretor && (!usuarioLogadoEhDelegado || !usuarioLogadoBd.Regional.Equals(regional)))
                return RetornaJsonDeAlerta("Usuário logado não é diretor ou delegado da regional em edição.");
            
            if (Request.Files.Count == 0)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var file = Request.Files[0];
            if (file == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var actualFileName = file.FileName;

            try
            {
                var path = Server.MapPath("~/Content/Aplicacao/images/regionais/" + codigoRegional);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!string.IsNullOrEmpty(regional.NomeArquivoLogo))
                {
                    // remover logotipo atual!!
                    var pathImagemAnterior = Path.Combine(path, regional.NomeArquivoLogo);
                    System.IO.File.Delete(pathImagemAnterior);
                }

                var pathCompleto = Path.Combine(path, actualFileName);
                file.SaveAs(pathCompleto);

                regional.NomeArquivoLogo = actualFileName;
            }
            catch (Exception)
            {
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Erro ao anexar arquivo"));
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Logotipo {0} salvo com sucesso", actualFileName), "Logotipo salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a regional de id {2}", _user.Name, prefixoOperacao, id);

            var regional = _cadastroRegionais.Busca(id);

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            var usuarioLogadoEhAdmin = usuarioLogado.IsInRole("ADMIN");
            var usuarioLogadoEhDiretor = usuarioLogado.IsInRole("DIRETOR");

            if (usuarioLogadoBd == null)
                return RetornaJsonDeAlerta("Usuário logado não encontrado no BD");

            if (!usuarioLogadoEhAdmin && !usuarioLogadoEhDiretor)
                return RetornaJsonDeAlerta("Usuário logado não é diretor.");

            regional.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Regional <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), regional.Codigo, regional.Codigo, prefixoOperacao),
                string.Format("Regional {0}ativada", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Regional não salva");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Regional regional)
        {
            var nomeUpper = regional.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroRegionais
                .BuscaTodos()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != regional.Codigo);

            return temComMesmoNome;
        }
    }
}