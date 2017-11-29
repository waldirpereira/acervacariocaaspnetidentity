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
using Acerva.Utils;
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

        private const string TextoInstrucoesPagSeguro =
            "O pagamento deve ser realizado no site do Pag Seguro. Você deve ter recebido um e-mail deles com o link para o pagamento. No site, você pode optar por efetuar o pagamento com cartão de crédito, débito ou boleto. Também é possível parcelar, com juros, diretamente com o Pag Seguro.<br/>" +
            "<br/>" +
            "Caso não encontre o e-mail do Pag Seguro com o link da cobrança:<br/>" +
            "<br/>" +
            "1. Olhar a caixa de SPAM e procurar por um e-mail do PagSeguro (pagamento@pagseguro.com.br), com o assunto \"Solicitação de pagamento de Acerva Carioca\"<br/>" +
            "<br/>" +
            "2. Entre na conta do Pag Seguro associada ao seu e-mail cadastrado na ACervA e procure pela cobrança pendente.Ela aparece logo na tela principal, na lista das últimas transações(clique em extrato de transações para ter acesso à cobrança)<br/>" +
            "<br/>" +
            "3. Em 2015, o Pag Seguro alterou a sua política de relacionamento com os usuários.As pessoas cadastradas na época e que não aceitaram os novos termos tiveram o acesso ao Pag Seguro bloqueado. Se você recebeu o e-mail, mas não consegue entrar no Pag Seguro para efetuar o pagamento, este pode ser o motivo.Neste caso, o problema deve ser resolvido diretamente com o Pag Seguro.<br/>" +
            "<br/>" +
            "4. Nós não geramos boletos. Geramos cobranças através do PagSeguro, que admite diversas formas de pagamento, como cartão de crédito, cartão de débito, transferência e boleto.Caso você opte por boleto, o Pag Seguro vai gerar o boleto com uma data de vencimento. Se o pagamento não for efetuado até a data informada por eles, você pode solicitar no e-mail financeiro@acervacarioca.com.br a geração de uma segunda via do boleto, com nova data de vencimento.<br/>" +
            "<br/>" +
            "5. Em alguns casos, se ao efetuar o pagamento ocorrer um erro (por exemplo, os dados do cartão de crédito forem informados de forma incorreta), a cobrança pode ser cancelada pelo Pag Seguroautomaticamente e novas tentativas de pagamento serão recusadas.Nestes casos, por favor envie um e-mail para financeiro @acervacarioca.com.br e solicite a geração de uma nova cobrança";


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
        public ActionResult BuscaParaListagem(bool incluiCancelados = false)
        {
            var listaUsuariosJson = _cadastroUsuarios.BuscaParaListagem(incluiCancelados).ToList();

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

            var listaUsuariosViewModel = listaUsuariosJson.Select(Mapper.Map<UsuarioListagemViewModel>);
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
            var ufsJson = _cadastroUsuarios.BuscaUfs()
                .Select(Mapper.Map<UfViewModel>);

            return new JsonNetResult(new
            {
                Regionais = regionaisJson,
                IdUsuarioLogado = usuarioLogado.Identity != null ? usuarioLogado.Identity.GetUserId() : null,
                RegionalDoUsuarioLogado = regionalDoUsuarioLogadoJson,
                UsuarioLogadoEhAdmin = usuarioLogadoEhAdmin,
                UsuarioLogadoEhDiretor = usuarioLogadoEhDiretor,
                UsuarioLogadoEhDelegado = usuarioLogadoEhDelegado,
                Papeis = papeisJson,
                Ufs = ufsJson
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
        public async Task<ActionResult> Salva([JsonBinder]UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está salvando o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var ehNovo = string.IsNullOrEmpty(usuarioViewModel.Id);
            var usuario = ehNovo ? new Usuario() : _cadastroUsuarios.Busca(usuarioViewModel.Id);

            var idUsuarioQueIndicouAnterior = usuario.UsuarioIndicacao?.Id;

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

            var textoSufixoMensagemSucesso = "";

            if (ehNovo)
                _cadastroUsuarios.SalvaNovo(usuario);
            else
            {
                if (idUsuarioQueIndicouAnterior != usuario.UsuarioIndicacao?.Id &&
                    usuario.Status == StatusUsuario.AguardandoIndicacao)
                {
                    var codigoConfirmacaoIndicacao = await UserManager.GenerateUserTokenAsync("confirmacao", usuario.Id);
                    usuario.IndicacaoHash = codigoConfirmacaoIndicacao;
                    var mensagemEmailIndicacao = _helper.MontaMensagemIndicacao(usuario, codigoConfirmacaoIndicacao, Url, Request);

                    textoSufixoMensagemSucesso = String.Format("<br/><br/>Um novo e-mail foi enviado ao usuário {0}, pois houve alteração do usuário que indicou.", 
                        usuario.UsuarioIndicacao.Name);

                    await UserManager.SendEmailAsync(usuario.UsuarioIndicacao.Id, "Confirme a indicação para ACervA Carioca", mensagemEmailIndicacao);
                }
            }

            _helper.SalvaFoto(usuario.Id, usuarioViewModel.FotoBase64, HttpContext);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Usuário <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso.{3}", Url.Action("Index"), usuario.Id, usuario.Name, textoSufixoMensagemSucesso),
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

        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult ReenviarEmailIndicacao([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está reenviando e-mail de indicação para o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            return RedirectToAction("ResendDesignationEmail", "Account", new
            {
                userId = usuarioViewModel.Id
            });
        }

        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult ReenviarEmailConfirmacaoEmail([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está reenviando e-mail de confirmação de e-mail para o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            return RedirectToAction("EnviaEmailConfirmacaoEmail", "Account", new
            {
                userId = usuarioViewModel.Id
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

            await ProcessaConfirmacaoPagamentoUsuario(usuario);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado teve seu pagamento confirmado com sucesso", "Pagamento confirmado");
            return new JsonNetResult(new { growlMessage });
        }

        private async Task ProcessaConfirmacaoPagamentoUsuario(Usuario usuario)
        {
            var enviaEmailBoasVindas = usuario.Status == StatusUsuario.AguardandoPagamentoAnuidade;

            usuario.Status = StatusUsuario.Ativo;
            usuario.Matricula = string.IsNullOrEmpty(usuario.Matricula)
                ? _cadastroUsuarios.PegaProximaMatricula()
                : usuario.Matricula;
            usuario.DataAdmissao = DateTime.Today;

            if (enviaEmailBoasVindas)
            {
                await EnviaEmailParaAdministrativo($"Olá Administrativo,<br/><br/>A pessoa {usuario.Name} acabou de se tornar ATIVA. Favor adicioná-la ao COCECA!",
                    $"Adicionar ao COCECA: {usuario.Email}");

                var mensagemDelegado = "Olá delegado,<br/><br/>" +
                                       $"A pessoa '{usuario.Name}' (e-mail: {usuario.Email} - celular: {usuario.PhoneNumber}) acabou de se tornar associada!";
                await EnviaEmailParaDelegadosDaRegional(usuario, mensagemDelegado,
                    $"ACervA Carioca - novo associado: {usuario.Name}({usuario.Email})");
                await EnviaEmailDeBoasVindas(usuario);
            }

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Pagamento confirmado", string.Format("Olá {0},<br/><br/>" +
                                 "Acabamos de confirmar que o seu pagamento foi realizado.<br/>" +
                                 "Você agora é um(a) associado(a) ativo(a) na ACervA Carioca.<br/><br/>Muito obrigado!", usuario.Name));

            _cadastroUsuarios.Atualiza(usuario);
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

        private async Task EnviaEmailParaAdministrativo(string mensagem, string titulo)
        {
            var identityMessage = new IdentityMessage
            {
                Destination = "administrativo@acervacarioca.com.br",
                Subject = titulo,
                Body = mensagem
            };

            await UserManager.EmailService.SendAsync(identityMessage);
        }

        private async Task EnviaEmailParaDelegadosDaRegional(Usuario usuario, string mensagem, string titulo)
        {

            var delegados = _cadastroUsuarios.BuscaDelegadosDaRegional(usuario.Regional.Codigo).ToList();

            delegados.ForEach(async d =>
            {
                var identityMessage = new IdentityMessage
                {
                    Destination = d.Email,
                    Subject = titulo,
                    Body = mensagem
                };

                await UserManager.EmailService.SendAsync(identityMessage);
            });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> ConfirmaPagamentoSelecionados([JsonBinder] IEnumerable<string> idsUsuarios)
        {
            var listaIdsUsuarios = idsUsuarios.ToList();
            Log.InfoFormat("Usuário está confirmando pagamento de anuidade dos associados de códigos {0}",
                listaIdsUsuarios.Aggregate((x, y) => x + ", " + y));

            foreach (var userId in listaIdsUsuarios)
            {
                var usuario = _cadastroUsuarios.Busca(userId);
                if (usuario.Status != StatusUsuario.AguardandoPagamentoAnuidade &&
                    usuario.Status != StatusUsuario.AguardandoRenovacao)
                {
                    return RetornaJsonDeAlerta(string.Format("Associado {0} não está aguardando confirmação de pagamento!", usuario.Name));
                }

                await ProcessaConfirmacaoPagamentoUsuario(usuario);
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associados tiveram seus pagamentos confirmados com sucesso", "Pagamentos confirmados");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> EnviaEmailBoasVindasNaListaSelecionados([JsonBinder] IEnumerable<string> idsUsuarios)
        {
            var listaIdsUsuarios = idsUsuarios.ToList();
            Log.InfoFormat("Usuário está enviando e-mail de boas vindas no COCECA para os associados de códigos {0}",
                listaIdsUsuarios.Aggregate((x, y) => x + ", " + y));

            var emailBoasVindasNaLista = "A ACervA Carioca dá as boas-vindas aos novos associados:<br /><br /><ul>";

            var listaUsuarios = new List<Usuario>();
            listaIdsUsuarios.ForEach(id => listaUsuarios.Add(_cadastroUsuarios.Busca(id)));

            listaUsuarios
                .Where(u => !u.EmailBoasVindasListaEnviado)
                .OrderBy(u => u.Matricula)
                .ToList()
                .ForEach(usuario =>
            {
                if (usuario.Status != StatusUsuario.Ativo)
                {
                    throw new Exception(string.Format("Associado {0} não está ativo!", usuario.Name));
                }

                emailBoasVindasNaLista += "<li>";
                emailBoasVindasNaLista += string.Format("<b>{0} - {1} ({2})</b>", usuario.Matricula, usuario.Name,
                    usuario.Regional.Nome);
                if (!string.IsNullOrEmpty(usuario.Experiencia))
                    emailBoasVindasNaLista += string.Format("<br />{0}", usuario.Experiencia);
                emailBoasVindasNaLista += "</li>";

                usuario.EmailBoasVindasListaEnviado = true;
            });

            emailBoasVindasNaLista += "</ul>" +
                "Apareçam em nossos encontros, tanto aos abertos ao público quanto aos fechados para associados, para conhecer e trocar experiências " +
                "com os demais associados. A agenda de eventos está disponível no link abaixo:<br /><br />http://www.acervacarioca.com.br/agenda";

            var identityMessage = new IdentityMessage
            {
                Destination = "coceca@googlegroups.com",
                Subject = "Boas vindas aos novos associados!",
                Body = emailBoasVindasNaLista
            };

            await UserManager.EmailService.SendAsync(identityMessage);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associados foram incluídos no e-mail de boas vindas na lista (COCECA) com sucesso!", "E-mail de boas vindas enviado com sucesso!");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> CobrancaGerada([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está confirmando que a cobrança foi gerada para o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.Ativo &&
                usuario.Status != StatusUsuario.Novo)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ativo ou novo!", usuario.Name));
            }

            usuario.Status = usuario.Status == StatusUsuario.Ativo ? StatusUsuario.AguardandoRenovacao : StatusUsuario.AguardandoPagamentoAnuidade;

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Cobrança gerada", string.Format("Olá {0},<br/><br/>" +
                                 "Acabamos de gerar uma cobrança em seu nome.<br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.<br/><br/>{2}", 
                                 usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status), TextoInstrucoesPagSeguro));

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Pretendente teve sua cobrança confirmada como gerada com sucesso", "Cobrança gerada confirmada");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> CobrancaGeradaSelecionados([JsonBinder] IEnumerable<string> idsUsuarios)
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

                await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Cobrança gerada", string.Format("Olá {0},<br/><br/>" +
                                     "Acabamos de gerar uma cobrança em seu nome.<br/>" +
                                     "Seu status agora é {1} na ACervA Carioca.<br/><br/>{2}", 
                                     usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status), TextoInstrucoesPagSeguro));
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Pretendentes tiveram suas cobranças confirmadas como geradas com sucesso", "Cobranças geradas confirmadas");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> CancelaUsuario([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está cancelando o registro do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);
            usuario.Status = StatusUsuario.Cancelado;

            await ProcessaCancelamentoUsuario(usuario);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado cancelado com sucesso", "Cancelamento confirmado");
            return new JsonNetResult(new { growlMessage });
        }

        private async Task ProcessaCancelamentoUsuario(Usuario usuario)
        {
            await EnviaEmailParaAdministrativo($"Olá Administrativo,<br/><br/>A pessoa {usuario.Name} acabou de se tornar CANCELADA. Favor removê-la ao COCECA!",
                $"Retirar do COCECA: {usuario.Email}");

            var mensagemDelegado = "Olá delegado,<br/><br/>" +
                                       $"A pessoa '{usuario.Name}' (e-mail: {usuario.Email} - celular: {usuario.PhoneNumber}) acabou de ser CANCELADA!";
            await EnviaEmailParaDelegadosDaRegional(usuario, mensagemDelegado,
                    $"ACervA Carioca - cancelamento de associado: {usuario.Name}({usuario.Email} - {usuario.PhoneNumber})");

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Cancelamento", string.Format("Olá {0},<br/><br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> ReativaUsuario([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está reativando o registro do associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.Cancelado)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está cancelado!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Novo;

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Reativação", string.Format("Olá {0},<br/><br/>" +
                                 "Acabamos de reativar seu registro.<br/>" +
                                 "Seu status agora é {1} na ACervA Carioca. Em breve o financeiro enviará a cobrança ou confirmará sua adimplência.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado reativado com sucesso", "Reativação confirmada");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> VoltaUsuarioParaNovo([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para novo o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoPagamentoAnuidade)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ag. pagamento de anuidade!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Novo;

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Retorno para ag. geração de cobrança de anuidade", string.Format("Olá {0},<br/><br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado retornado para Novo com sucesso", "Retorno para Novo");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> VoltaUsuarioParaAtivo([JsonBinder] UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está voltando para ativo o associado {0} de código {1} e email {2}",
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var usuario = _cadastroUsuarios.Busca(usuarioViewModel.Id);

            if (usuario.Status != StatusUsuario.AguardandoRenovacao)
            {
                return RetornaJsonDeAlerta(string.Format("Associado {0} não está ag. renovação!", usuario.Name));
            }

            usuario.Status = StatusUsuario.Ativo;

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Retorno para status ativo", string.Format("Olá {0},<br/><br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success, "Associado retornado para Ativo com sucesso", "Retorno para Ativo");
            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
        public async Task<ActionResult> VoltaUsuarioParaAguardandoConfirmacaoEmail([JsonBinder] UsuarioViewModel usuarioViewModel)
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

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Retorno para status ag. confirmação de e-mail", string.Format("Olá {0},<br/><br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));

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

            await UserManager.SendEmailAsync(usuario.Id, "ACervA Carioca - Retorno para status ag. indicação", string.Format("Olá {0},<br/><br/>" +
                                 "Seu status agora é {1} na ACervA Carioca.", usuario.Name, NomeExibicaoAttribute.GetNome(usuario.Status)));

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

        public ActionResult BuscaHistoricoStatus(string id)
        {
            var listaHistoricoJson = _cadastroUsuarios.BuscaHistoricoStatus(id)
                .OrderBy(h => h.DataHora)
                .Select(Mapper.Map<HistoricoStatusUsuarioViewModel>);

            return new JsonNetResult(listaHistoricoJson);
        }
    }
}