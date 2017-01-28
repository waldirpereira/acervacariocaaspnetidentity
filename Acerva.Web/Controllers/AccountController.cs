using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Acerva.Web.Models;
using Acerva.Web.Models.Home;
using AutoMapper;
using Facebook;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    public class AccountController : ApplicationBaseController
    {
        private readonly IValidator<Usuario> _validator;
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly IIdentity _user;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AccountController(IValidator<Usuario> validator, ICadastroUsuarios cadastroUsuarios, IPrincipal user) : base(cadastroUsuarios)
        {
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _user = user.Identity;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    await SendEmailConfirmationTokenAsync(user.Id, "Confirmação de seu e-mail foi reenviada.");

                    ViewBag.errorMessage = "Você precisa confirmar seu e-mail para se logar.<br/><br/>Um novo código foi enviado para seu e-mail.";
                    return View("Error");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", @"Tentativa inválida de login.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", @"Código inválido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAjaxAntiForgeryToken]
        public async Task<ActionResult> Register([JsonBinder]UsuarioNovoViewModel usuarioViewModel)
        {
            if (!ModelState.IsValid)
                return View(usuarioViewModel);

            if (_cadastroUsuarios.BuscaPeloEmail(usuarioViewModel.Email) != null)
            {
                return RetornaJsonDeRetorno("Erro ao registrar associado",
                        "Este e-mail já está cadastrado em nossa base. Utilize o \"Esqueci minha senha\" para recuperar seu acesso.");
            }

            var ehNovo = string.IsNullOrEmpty(usuarioViewModel.Id);
            var usuario = ehNovo ? new Usuario() : _cadastroUsuarios.Busca(usuarioViewModel.Id);

            Mapper.Map(usuarioViewModel, usuario);
            if (ehNovo)
            {
                usuario.Id = Guid.NewGuid().ToString();
                usuario.CreationDate = DateTime.Now;
                usuario.PasswordHash = UserManager.PasswordHasher.HashPassword(usuarioViewModel.Password);
                usuario.SecurityStamp = Guid.NewGuid().ToString();
                usuario.Status = StatusUsuario.AguardandoConfirmacaoEmail;
            }

            var validacao = _validator.Validate(usuario);
            if (!validacao.IsValid)
            {
                return RetornaJsonDeRetorno("Erro ao registrar associado", validacao.GeraListaHtmlDeValidacoes());
            }

            if (_cadastroUsuarios.ExisteComMesmoNome(usuario))
            {
                return RetornaJsonDeRetorno("Erro ao registrar associado",string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um usuário com o nome {0:unsafe}", usuario.Name));
            }

            var caminhoFotos = HttpContext.Server.MapPath("~/Content/Aplicacao/images/fotos");
            
            try
            {
                _cadastroUsuarios.BeginTransaction();

                if (ehNovo)
                    _cadastroUsuarios.SalvaNovo(usuario);

                _cadastroUsuarios.Commit();
            }
            catch
            {
                _cadastroUsuarios.Rollback();
            }

            Log.InfoFormat("E-mail de confirmação de conta sendo enviado para '{0}'", usuario.Email);
            await SendEmailConfirmationTokenAsync(usuario.Id, "Confirme seu e-mail");

            return new JsonNetResult("OK");
        }

        private static ActionResult RetornaJsonDeRetorno(string titulo, string mensagem, 
            GrowlMessageSeverity growlSeverity = GrowlMessageSeverity.Warning, int statusCode = JsonNetResult.HttpBadRequest)
        {
            var growlMessage = new GrowlMessage(growlSeverity, mensagem, titulo);

            return new JsonNetResult(new { growlMessage }, statusCode: statusCode);
        }


        [Transacao]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmDesignation(string userId, string code)
        {
            if (userId == null)
            {
                ViewBag.errorMessage = "Não foi fornecido id para o usuário a ser ativado.";
                return View("Error");
            }

            var usuario = _cadastroUsuarios.Busca(userId);

            var valido = ValidaAlteracaoIndicacao(usuario, code);
            if (!valido)
                return View("Error");

            usuario.IndicacaoHash = null;
            usuario.Status = StatusUsuario.Novo;

            var mensagem = string.Format("Olá Financeiro,<br/><br/>" +
                                             "A pessoa {0} acabou de ter sua indicação confirmada.<br/><br/>" +
                                                 "Obrigado,<br/>ACervA Carioca", usuario.Name);

            var identityMessage = new IdentityMessage
            {
                Destination = "financeiro@acervacarioca.com.br",
                Subject = string.Format("Novo associado: {0}", usuario.Name),
                Body = mensagem
            };

            _cadastroUsuarios.Atualiza(usuario);

            await UserManager.EmailService.SendAsync(identityMessage);

            if (code != null)
                return View();

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                "Indicação confirmada com sucesso!", "Indicação confirmada");

            return new JsonNetResult(new { growlMessage });
        }

        private bool ValidaAlteracaoIndicacao(Usuario usuario, string code)
        {
            if (usuario == null)
            {
                ViewBag.errorMessage = "Não foi encontrado o usuário.";
                return false;
            }

            if (code == null && !_user.IsAuthenticated)
            {
                ViewBag.errorMessage = "Código de confirmação não informado.";
                return false;
            }

            var estaAutenticado = _user.IsAuthenticated;
            if (estaAutenticado)
            {
                var usuarioLogado = HttpContext.User;
                var usuarioLogadoBd = _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId());
                var ehUsuarioQueIndicou = usuario.UsuarioIndicacao != null && usuarioLogadoBd.Id == usuario.UsuarioIndicacao.Id;
                var ehAdminOuDiretor = usuarioLogado.IsInRole("ADMIN") || usuarioLogado.IsInRole("DIRETOR");
                var ehDelegadoDaRegionalDoAssociadoATerIndicacaoConfirmada = usuarioLogado.IsInRole("DELEGADO") &&
                                                                             usuarioLogadoBd.Regional.Codigo == usuario.Regional.Codigo;

                if (code == null)
                {
                    if (ehUsuarioQueIndicou || ehAdminOuDiretor || ehDelegadoDaRegionalDoAssociadoATerIndicacaoConfirmada)
                        return true;

                    ViewBag.errorMessage = "Código de confirmação não informado.";
                    return false;
                }
            }

            if (usuario.IndicacaoHash == code)
                return true;

            ViewBag.errorMessage = "Código de confirmação inválido.";
            return false;
        }

        [Transacao]
        [AllowAnonymous]
        public async Task<ActionResult> DenyDesignation(string userId, string code)
        {
            if (userId == null)
            {
                ViewBag.errorMessage = "Não foi fornecido id para o usuário.";
                return View("Error");
            }

            var usuario = _cadastroUsuarios.Busca(userId);

            var valido = ValidaAlteracaoIndicacao(usuario, code);
            if (!valido)
                return View("Error");

            usuario.IndicacaoHash = null;
            usuario.Status = StatusUsuario.Cancelado;

            var mensagem = string.Format("Olá {0},<br/><br/>" +
                                             "{1} acabou de informar que não indicou você para a ACervA Carioca.<br/><br/>" +
                                                 "Atenciosamente,<br/>ACervA Carioca", usuario.Name, HttpContext.User.Identity.Name);

            _cadastroUsuarios.Atualiza(usuario);

            await UserManager.SendEmailAsync(usuario.Id, "Indicação recusada", mensagem);

            if (code != null)
                return View();

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                "Indicação recusada com sucesso!", "Indicação recusada");

            return new JsonNetResult(new { growlMessage });
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code, bool isJsonReturn = false)
        {
            if (userId == null || (code == null && !isJsonReturn))
            {
                return isJsonReturn ? RetornaJsonDeRetorno("Erro ao confirmar e-mail", "Identificador do associado não foi informado") : View("Error");
            }

            var usuarioHibernate = _cadastroUsuarios.Busca(userId);
            var codigoConfirmacaoIndicacao = await UserManager.GenerateUserTokenAsync("confirmacao", usuarioHibernate.Id);

            var result = new IdentityResult();
            try
            {
                if (!isJsonReturn)
                {
                    result = await UserManager.ConfirmEmailAsync(usuarioHibernate.Id, code);
                }

                _cadastroUsuarios.BeginTransaction();

                usuarioHibernate.Status = StatusUsuario.AguardandoIndicacao;
                usuarioHibernate.IndicacaoHash = codigoConfirmacaoIndicacao;
                usuarioHibernate.EmailConfirmed = true;
                
                _cadastroUsuarios.Atualiza(usuarioHibernate);

                _cadastroUsuarios.Commit();
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                ViewBag.errorMessage = ioe.Message;
                return isJsonReturn ? RetornaJsonDeRetorno("Erro ao confirmar e-mail", ViewBag.errorMessage) : View("Error");
            }
            catch (Exception)
            {
                _cadastroUsuarios.Rollback();
            }

            if (result.Succeeded || isJsonReturn)
            {
                if (usuarioHibernate.UsuarioIndicacao == null)
                {
                    ViewBag.errorMessage = "Sem associado que indicou!";
                    return isJsonReturn ? RetornaJsonDeRetorno("Erro ao confirmar e-mail", ViewBag.errorMessage) : View("Error");
                }

                var callbackUrlConfirmacao = Url.Action("ConfirmDesignation", "Account",
                   new { userId = usuarioHibernate.Id, code = codigoConfirmacaoIndicacao }, protocol: Request.Url.Scheme);

                var callbackUrlRecusa = Url.Action("DenyDesignation", "Account",
                   new { userId = usuarioHibernate.Id, code = codigoConfirmacaoIndicacao }, protocol: Request.Url.Scheme);

                var mensagem = string.Format("Olá {0},<br/><br/>" +
                                             "Recebemos um novo pedido de associação à ACervA Carioca, de {1}, da regional {2}. Esta pessoa mencionou ter sido indicada por você.<br/><br/>" +
                                             "Por favor confirme esta indicação clicando <a href=\"{3}\">aqui</a>.<br/><br/>" +
                                                 "Se o link para confirmar não funcionar, copie o endereço abaixo e cole em seu navegador.<br/><br/>{3}<br/><br/>" +
                                                 "Caso não tenha sido você que indicou esta pessoa, por favor recuse a indicação clicando <a href=\"{4}\">aqui</a>.<br/><br/>" +
                                                 "Se o link para recusar não funcionar, copie o endereço abaixo e cole em seu navegador.<br/><br/>{4}<br/><br/>" +
                                                 "Obrigado,<br/>ACervA Carioca",
                        usuarioHibernate.UsuarioIndicacao.Name, usuarioHibernate.Name, usuarioHibernate.Regional.Nome, callbackUrlConfirmacao, callbackUrlRecusa);
                await UserManager.SendEmailAsync(usuarioHibernate.UsuarioIndicacao.Id, "Confirme a indicação para ACervA Carioca", mensagem);

                return isJsonReturn ? RetornaJsonDeRetorno("E-mail confirmado com sucesso", "Associado teve seu e-mail confirmado com sucesso", 
                    GrowlMessageSeverity.Success, JsonNetResult.HttpOk) : View();
            }

            // If we got this far, something failed.
            AddErrors(result);
            ViewBag.errorMessage = "Confirmação de e-mail falhou.";
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            // If we got this far, something failed, redisplay form
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                ViewBag.errorMessage = "Usuário não encontrado ou ainda não teve o e-mail confirmado.";
                return View("Error");
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            var mensagem = string.Format("Olá, {0}.<br/><br/>" +
                                         "Uma recuperação de senha foi requisitada para seu e-mail {1}.<br/><br/>" +
                                         "Clique <a href=\"{2}\">aqui</a> ou copie e cole o endereço abaixo em seu navegador para prosseguir com a recuperação de sua senha:<br/><br/>" +
                                         "{2}<br/><br/>" +
                                         "Obrigado por sua atenção, {3}" +
                                         "{4}", user.Name, user.Email, callbackUrl, Sistema.NomeSistema, Url.Action("Index", "Home"));

            await UserManager.SendEmailAsync(user.Id, "Recuperação de senha", mensagem);
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe = false)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);

            var mensagem = string.Format("Por favor confirme seu e-mail de cadastro no {0} clicando <a href=\"{1}\">aqui</a>.<br/><br/>" +
                                             "Se o link não funcionar, copie o endereço abaixo e cole em seu navegador.<br/><br/>{1}<br/><br/>" +
                                             "Obrigado,<br/>Equipe {0}",
                    Sistema.NomeSistema, callbackUrl);
            await UserManager.SendEmailAsync(userID, "Confirme seu e-mail", mensagem);

            return callbackUrl;
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                Log.Info("ExternalLoginCallback - loginInfo NULL");
                return RedirectToAction("Login");
            }

            Log.InfoFormat("ExternalLoginCallback - loginInfo = {0}", loginInfo.Email);

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    var identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                    var accessToken = identity.FindFirstValue("FacebookAccessToken");
                    if (accessToken != null)
                    {
                        var fb = new FacebookClient(accessToken);
                        dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field    

                        var user = await UserManager.FindByNameAsync(myInfo.email);
                        if (user != null)
                        {
                            var addLoginResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                            if (addLoginResult.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                    }
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new Usuario { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}