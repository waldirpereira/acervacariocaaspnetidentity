using System;

namespace Acerva.Modelo
{
    public class Partida
    {
        public virtual int Codigo { get; set; }
        public virtual Rodada Rodada { get; set; }
        public virtual Equipe EquipeMandante { get; set; }
        public virtual int? PlacarMandante { get; set; }
        public virtual Equipe EquipeVisitante { get; set; }
        public virtual int? PlacarVisitante { get; set; }
        public virtual DateTime? DataHora { get; set; }
        public virtual bool Terminada { get; set; }

        public virtual DateTime? Data
        {
            get { return DataHora.HasValue ? DataHora.Value.Date : (DateTime?) null ; }
            set
            {
                if (value == null)
                {
                    DataHora = null;
                    return;
                }
                
                DataHora = value.Value;
                
                if (Horario != null)
                    DataHora.Value.AddHours(Horario.Hour).AddMinutes(Horario.Minute);
            }
        }

        public virtual Time Horario
        {
            get
            {
                if (!DataHora.HasValue)
                    return null;

                return new Time
                {
                    Hour = DataHora.Value.Hour,
                    Minute = DataHora.Value.Minute
                }
                ;
            }
            set
            {
                if (value == null )
                    return;
                if (!DataHora.HasValue)
                    DataHora = default(DateTime).Date;

                DataHora.Value.AddHours(value.Hour).AddMinutes(value.Minute);
            }
        }
    }
}