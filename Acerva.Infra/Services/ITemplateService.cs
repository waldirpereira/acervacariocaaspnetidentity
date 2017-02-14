namespace Acerva.Infra.Services
{
    public interface ITemplateService
    {
        string GeraHtmlEmail(string caminhoCompletoTemplate, string titulo, string corpo);
    }
}