using System;

namespace Acerva.Web.Models.CadastroAcervas
{
    public class RegraViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual int Pontuacao { get; set; }
        public virtual DateTime DataHoraAlteracao { get; set; }
        public virtual CriterioViewModel Criterio { get; set; }
    }
}