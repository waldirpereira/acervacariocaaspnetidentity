using System;
using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Pergunta : IEquatable<Pergunta>
    {
        public const int TamanhoMaximoTitulo = 200;
        public const int TamanhoMaximoTexto = 2000;

        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string Texto { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual Votacao Votacao { get; set; }

        private IEnumerable<Opcao> _opcoes = new List<Opcao>();
        public virtual IEnumerable<Opcao> Opcoes
        {
            get { return _opcoes; }
            set { _opcoes = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Pergunta;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Pergunta other)
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
            return Titulo;
        }
    }
}
