namespace Acerva.Modelo
{
    public class AnexoNoticia
    {
        public const int TamanhoMaximoTitulo = 300;
        public const int TamanhoMaximoNomeArquivo = 256;

        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string NomeArquivo { get; set; }
        public virtual Noticia Noticia { get; set; }
    }
}
