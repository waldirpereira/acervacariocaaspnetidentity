using System;

namespace Acerva.Modelo
{
    public class Palpite
    {
        public virtual int Codigo { get; set; }
        public virtual Partida Partida { get; set; }
        public virtual Participacao Participacao { get; set; }
        public virtual int? PlacarMandante { get; set; }
        public virtual int? PlacarVisitante { get; set; }
        public virtual DateTime? DataHoraPalpite { get; set; }
        public virtual int? Pontuacao { get; set; }
        public virtual DateTime? DataHoraPontuacao { get; set; }
        public virtual Criterio Criterio { get; set; }
    }
}