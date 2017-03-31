using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using Acerva.Modelo;
using log4net;
using Microsoft.AspNet.Identity;

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
}