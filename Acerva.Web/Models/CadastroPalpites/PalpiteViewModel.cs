using System;

namespace Acerva.Web.Models.CadastroPalpites
{
    public class PalpiteViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual PartidaViewModel Partida { get; set; }
        public virtual int? PlacarMandante { get; set; }
        public virtual int? PlacarVisitante { get; set; }
        public virtual DateTime? DataHoraPalpite { get; set; }
        public virtual int? Pontuacao { get; set; }
        public virtual DateTime? DataHoraPontuacao { get; set; }
        public virtual CriterioViewModel Criterio { get; set; }
    }
}