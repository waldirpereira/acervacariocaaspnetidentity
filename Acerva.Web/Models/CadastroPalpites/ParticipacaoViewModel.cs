using System;
using System.Collections.Generic;

namespace Acerva.Web.Models.CadastroPalpites
{
    public class ParticipacaoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual bool AcervaAtivo { get; set; }
        public virtual int CodigoAcerva { get; set; }
        public virtual string NomeAcerva { get; set; }
        public virtual int CodigoRegional { get; set; }
        public virtual string NomeRegional { get; set; }
        public virtual IdentityUserViewModel Usuario { get; set; }
        public virtual int PontuacaoInicial { get; set; }
        public virtual DateTime? DataHoraInclusao { get; set; }
        public virtual int PontuacaoAtual { get; set; }
        public virtual int Posicao { get; set; }
        public virtual ICollection<PalpiteViewModel> Palpites { get; set; }
    }
}