using System;

namespace Acerva.Modelo
{
    public class Opcao : IEquatable<Opcao>
    {
        public const int TamanhoMaximoTexto = 2000;

        public virtual int Codigo { get; set; }
        public virtual Pergunta Pergunta { get; set; }
        public virtual string Texto { get; set; }
        public virtual bool Ativo { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Opcao;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Opcao other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Codigo == Codigo;
        }

        public override int GetHashCode()
        {
            return Codigo;
        }

        public override string ToString()
        {
            return Texto.Length <= 30 ? Texto : string.Format("{0}...", Texto.Substring(0, 30));
        }
    }
}
