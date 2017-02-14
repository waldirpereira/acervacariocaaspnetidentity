using System;
using System.IO;

namespace Acerva.Infra.Services
{
    public class TemplateService : ITemplateService
    {
        public string GeraHtmlEmail(string caminhoCompletoTemplate, string titulo, string corpo)
        {
            var template = "%TITULO%<br /><hr />%CORPO%<hr /><br />ACervA Carioca %ANO%";

            if (File.Exists(caminhoCompletoTemplate))
            {
                template = File.ReadAllText(caminhoCompletoTemplate);
            }

            return template
                .Replace("%CORPO%", corpo)
                .Replace("%TITULO%", titulo)
                .Replace("%ANO%", DateTime.Today.Year.ToString());
        }
    }
}
