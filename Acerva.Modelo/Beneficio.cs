using System;

namespace Acerva.Modelo
{
    public class Beneficio : IEquatable<Beneficio>
    {
        public const int TamanhoMaximoNome = 80;
        public const int TamanhoMaximoTextoHtml = 2000;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Beneficio;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Beneficio other)
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
            return Nome;
        }
    }
}
