using System;
using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Votacao : IEquatable<Votacao>
    {
        public const int TamanhoMaximoNome = 200;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual DateTime DataHoraInicio { get; set; }
        public virtual DateTime DataHoraFim { get; set; }
        public virtual bool Ativo { get; set; }

        private IEnumerable<Pergunta> _perguntas = new List<Pergunta>();
        public virtual IEnumerable<Pergunta> Perguntas
        {
            get { return _perguntas; }
            set { _perguntas = value; }
        }  
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var otherObj = obj as Votacao;
            return otherObj != null && Equals(otherObj);
        }

        public virtual bool Equals(Votacao other)
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
