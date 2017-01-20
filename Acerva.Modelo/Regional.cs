using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Regional
    {
        public const int TamanhoMaximoNome = 80;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }

        private ICollection<Equipe> _equipes = new List<Equipe>();
        public virtual ICollection<Equipe> Equipes
        {
            get { return _equipes; }
            set { _equipes = value; }
        }

        private ICollection<Rodada> _rodadas = new List<Rodada>();

        public virtual ICollection<Rodada> Rodadas
        {
            get { return _rodadas; }
            set { _rodadas = value; }
        }
    }
}
