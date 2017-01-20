using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Rodada
    {
        public const int TamanhoMaximoNome = 40;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual int Ordem { get; set; }
        public virtual Regional Regional { get; set; }

        private ICollection<Partida> _partidas = new List<Partida>();
        public virtual ICollection<Partida> Partidas
        {
            get { return _partidas; }
            set { _partidas = value; }
        }
    }
}