namespace Acerva.Web.Models.CadastroAcervas
{
    public class CriterioViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual int Ordem { get; set; }
        public virtual string Nome { get; set; }
        public virtual int PontuacaoPadrao { get; set; }
    }
}