namespace Acerva.Web.Models.CadastroNoticias
{
    public class NoticiaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual int? Ordem { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }
    }
}