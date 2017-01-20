namespace Acerva.Modelo
{
    public class Equipe
    {
        public const int TamanhoMaximoNome = 80;
        public const int TamanhoMaximoSigla = 20;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Sigla { get; set; }
        public virtual string Escudo { get; set; }
    }
}
