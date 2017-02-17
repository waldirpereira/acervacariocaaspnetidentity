using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
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
using Microsoft.AspNet.Identity.Owin;

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
        private readonly ICadastroArtigos _cadastroArtigos;
        private ApplicationUserManager _userManager;

        public UsuarioController(IValidator<Usuario> validator,
            ICadastroUsuarios cadastroUsuarios, ICadastroRegionais cadastroRegionais, UsuarioControllerHelper helper, ICadastroArtigos cadastroArtigos) : base(cadastroUsuarios)
        {
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _cadastroRegionais = cadastroRegionais;
            _helper = helper;
            _cadastroArtigos = cadastroArtigos;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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
                usuario.PasswordHash = UserManager.PasswordHasher.HashPassword(usuarioViewModel.Password);
                usuario.SecurityStamp = Guid.NewGuid().ToString();
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
        public async Task<ActionResult> ConfirmaPagamento([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está confirmando pagamento de anuidade do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoPagamentoAnuidade &&
                usuario.Status != StatusUsuario.AguardandoRenovacao)
            {
                return RetornaJsonDeAlerta("Associado não está aguardando confirmação de pagamento!");
            }

            var enviaEmailBoasVindas = usuario.Status == StatusUsuario.AguardandoPagamentoAnuidade;

            usuario.Status = StatusUsuario.Ativo;
            usuario.Matricula = string.IsNullOrEmpty(usuario.Matricula) ? _cadastroUsuarios.PegaProximaMatricula() : usuario.Matricula;

            if (enviaEmailBoasVindas)
            {
                await EnviaEmailParaAdministrativoIncluirNoCoceca(usuario);
                await EnviaEmailDeBoasVindas(usuario);
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado teve seu pagamento confirmado com sucesso", "Pagamento confirmado");
            return new JsonNetResult(new { growlMessage });
        }

        private async Task EnviaEmailDeBoasVindas(Usuario usuario)
        {
            var artigoMensagemBoasVindas = _cadastroArtigos.Busca(Artigo.CodigoArtigoBoasVindas);
            if (artigoMensagemBoasVindas == null)
            {
                Log.Error(string.Format("Mensagem de boas vindas não encontrada ao tentar enviar para {0}", usuario.Email));
                return;
            }

            var mensagemBoasVindas = artigoMensagemBoasVindas.TextoHtml
                .Replace("%NOME%", usuario.Name);
            await UserManager.SendEmailAsync(usuario.Id, "Bem-vindo à ACervA Carioca", mensagemBoasVindas);
        }

        private async Task EnviaEmailParaAdministrativoIncluirNoCoceca(Usuario usuario)
        {
            var mensagem = string.Format("Olá Administrativo,<br/><br/>" +
                                             "A pessoa {0} acabou de se tornar ATIVA. Favor adicioná-la ao COCECA!", usuario.Name);

            var identityMessage = new IdentityMessage
            {
                Destination = "administrativo@acervacarioca.com.br",
                Subject = string.Format("Adicionar ao COCECA: {0}", usuario.Email),
                Body = mensagem
            };

            _cadastroUsuarios.Atualiza(usuario);

            await UserManager.EmailService.SendAsync(identityMessage);
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
                if (usuario.Status != StatusUsuario.AguardandoPagamentoAnuidade &&
                    usuario.Status != StatusUsuario.AguardandoRenovacao)
                {
                    return RetornaJsonDeAlerta(string.Format("Associado {0} não está aguardando confirmação de pagamento!", usuario.Name));
                }

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

            if (usuario.Status != StatusUsuario.Ativo &&
                usuario.Status != StatusUsuario.Novo)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ativo ou novo!", usuario.Name));
            }

            usuario.Status = usuario.Status == StatusUsuario.Ativo ? StatusUsuario.AguardandoRenovacao :  StatusUsuario.AguardandoPagamentoAnuidade;

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

                if (usuario.Status != StatusUsuario.Ativo &&
                    usuario.Status != StatusUsuario.Novo)
                {
                    return RetornaJsonDeAlerta(string.Format("Associado {0} não está ativo ou novo!", usuario.Name));
                }

                usuario.Status = usuario.Status == StatusUsuario.Ativo ? StatusUsuario.AguardandoRenovacao : StatusUsuario.AguardandoPagamentoAnuidade;
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

            if (usuario.Status != StatusUsuario.Cancelado)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está cancelado!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Novo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado reativado com sucesso", "Reativação confirmada");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult VoltaUsuarioParaNovo([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para novo o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoPagamentoAnuidade)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ag. pagamento de anuidade!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Novo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado retornado para Novo com sucesso", "Retorno para Novo");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult VoltaUsuarioParaAtivo([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para ativo o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoRenovacao)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ag. renovação!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado retornado para Ativo com sucesso", "Retorno para Ativo");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public ActionResult VoltaUsuarioParaAguardandoConfirmacaoEmail([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para ag. confirmacao email o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoIndicacao)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ag. indicação!", usuario.Name));
            }

            usuario.Status = StatusUsuario.AguardandoConfirmacaoEmail;
            usuario.EmailConfirmed = false;

            return RedirectToAction("EnviaEmailConfirmacaoEmail", "Account", new
            {
                userId = usuarioViewModel.Id
            });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> VoltaUsuarioParaAguardandoIndicacao([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para ag. indicação o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.Novo)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está novo!", usuario.Name));
            }

            var codigoConfirmacaoIndicacao = await UserManager.GenerateUserTokenAsync("confirmacao", usuario.Id);

            usuario.IndicacaoHash = codigoConfirmacaoIndicacao;
            usuario.Status = StatusUsuario.AguardandoIndicacao;

            return RedirectToAction("EnviaEmailIndicacao", "Account", new
            {
                userId = usuarioViewModel.Id,
                codigoConfirmacaoIndicacao
            });
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