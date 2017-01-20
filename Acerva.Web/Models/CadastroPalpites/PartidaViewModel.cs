using System;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroPalpites
{
    public class PartidaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual EquipeViewModel EquipeMandante { get; set; }
        public virtual int? PlacarMandante { get; set; }
        public virtual EquipeViewModel EquipeVisitante { get; set; }
        public virtual int CodigoRodada { get; set; }
        public virtual string NomeRodada { get; set; }
        public virtual int OrdemRodada { get; set; }
        public virtual int? PlacarVisitante { get; set; }
        public virtual bool Terminada { get; set; }
        public virtual DateTime? Data { get; set; }
        public virtual Time Horario { get; set; }
    }
}