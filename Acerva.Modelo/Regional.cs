using System;

namespace Acerva.Modelo
{
    public class Regional : IEquatable<Regional>
    {
        public const int TamanhoMaximoNome = 80;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual string NomeArquivoLogo { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Regional;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Regional other)
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
