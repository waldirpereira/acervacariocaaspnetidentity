using System;
using System.Collections.Generic;

namespace Acerva.Web.Models.CadastroAcervas
{
    public class ParticipacaoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual IdentityUserViewModel Usuario { get; set; }
        public virtual int PontuacaoInicial { get; set; }
        public virtual DateTime? DataHoraInclusao { get; set; }
        public virtual int PontuacaoAtual { get; set; }
        public virtual int Posicao { get; set; }
        public virtual int PosicaoRodadaAnterior { get; set; }
        public int CodigoAcerva { get; set; }
    }
}