using System;
using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Aviao : IEquatable<Aviao>
    {
        public const int TamanhoMaximoNome = 80;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Aviao;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Aviao other)
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
