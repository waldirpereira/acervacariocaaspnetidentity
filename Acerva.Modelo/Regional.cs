namespace Acerva.Modelo
{
    public class Regional
    {
        public const int TamanhoMaximoNome = 80;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }
    }
}
