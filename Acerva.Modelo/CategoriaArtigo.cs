namespace Acerva.Modelo
{
    public class CategoriaArtigo
    {
        public const int TamanhoMaximoNome = 60;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }
    }
}
