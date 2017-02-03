namespace Acerva.Web.Models.CadastroRegionais
{
    public class RegionalViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual string NomeArquivoLogo { get; set; }
        public virtual bool Ativo { get; set; }
    }
}