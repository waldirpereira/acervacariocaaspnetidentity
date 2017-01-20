using System;

namespace Acerva.Modelo
{
    public class Regra
    {
        public virtual int Codigo { get; set; }
        public virtual int Pontuacao { get; set; }
        public virtual DateTime DataHoraAlteracao { get; set; }
        public virtual Criterio Criterio { get; set; }
        public virtual Acerva Acerva { get; set; }
    }
}