using System;

namespace Acerva.Modelo
{
    public class Resposta : IEquatable<Resposta>
    {
        public virtual int Codigo { get; set; }
        public virtual Opcao Opcao { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DateTime DataHora { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Resposta;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Resposta other)
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
            return string.Format("{0} respondeu {1}", Usuario.Id, Opcao);
        }
    }
}
