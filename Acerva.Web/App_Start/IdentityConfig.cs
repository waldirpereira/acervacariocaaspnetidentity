using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.WebPages;
using Acerva.Infra;
using Acerva.Modelo;
using Acerva.Modelo.Mapeamento;
using log4net;

namespace Acerva.Web
{
    public class EmailService : IIdentityMessageService
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MailSettingsSectionGroup _mailSettings;

        public EmailService()
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            _mailSettings = (MailSettingsSectionGroup)configuration.GetSectionGroup("system.net/mailSettings");
        }

        public Task SendAsync(IdentityMessage message)
        {
            var pathTemplates = HostingEnvironment.MapPath("~/Templates");
            var template = "%TITULO%<br /><hr />%CORPO%<hr /><br />ACervA Carioca %ANO%";

            if (pathTemplates != null)
            {
                var pathTemplateEmailPadrao = Path.Combine(pathTemplates, "email-padrao.html");
                if (File.Exists(pathTemplateEmailPadrao))
                {
                    template = File.ReadAllText(pathTemplateEmailPadrao);
                }
            }

            using (var smtp = new SmtpClient(_mailSettings.Smtp.Network.Host, Convert.ToInt32(_mailSettings.Smtp.Network.Port)))
            using (var email = new MailMessage())
            {
                email.To.Add(message.Destination);
                email.Subject = message.Subject;

                email.Body = template
                    .Replace("%CORPO%", message.Body)
                    .Replace("%TITULO%", message.Subject)
                    .Replace("%ANO%", DateTime.Today.Year.ToString());

                email.IsBodyHtml = true;
                email.From = new MailAddress(_mailSettings.Smtp.From, Sistema.NomeSistema);

                smtp.DeliveryMethod = _mailSettings.Smtp.DeliveryMethod;

                if (smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                    smtp.PickupDirectoryLocation = _mailSettings.Smtp.SpecifiedPickupDirectory.PickupDirectoryLocation;

                if (smtp.DeliveryMethod == SmtpDeliveryMethod.Network)
                {
                    var credentials = new NetworkCredential(_mailSettings.Smtp.Network.UserName, _mailSettings.Smtp.Network.Password);
                    smtp.Credentials = credentials;
                    smtp.EnableSsl = _mailSettings.Smtp.Network.EnableSsl;
                }

                Log.InfoFormat("E-mail '{0}' sendo enviado para {1} pelo host {2}:{3} ", message.Subject,
                    message.Destination, _mailSettings.Smtp.Network.Host, Convert.ToInt32(_mailSettings.Smtp.Network.Port));

                smtp.Send(email);
            }
            
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<Usuario>
    {
        public ApplicationUserManager(IUserStore<Usuario> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<Usuario>(new MySQLDatabase()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<Usuario>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Usuario>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<Usuario>
            {
                Subject = "SecurityCode",
                BodyFormat = "Seu código de segurança é {0}"
            });
            manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<Usuario>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(3)
                    };
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<Usuario, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public ApplicationSignInManager(UserManager<Usuario, string> userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Usuario user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }


        public async Task<SignInStatus> SignInAsync(string userName, string password, bool rememberMe)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null) return SignInStatus.Failure;

            if (await UserManager.IsLockedOutAsync(user.Id)) return SignInStatus.LockedOut;

            if (!await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }

                return SignInStatus.Failure;
            }

            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                return SignInStatus.RequiresVerification;
            }

            await base.SignInAsync(user, rememberMe, false);
            return SignInStatus.Success;
        }
    }
}
