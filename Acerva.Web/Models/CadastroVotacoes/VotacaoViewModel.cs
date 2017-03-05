using System;

namespace Acerva.Web.Models.CadastroVotacoes
{
    public class VotacaoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual DateTime DataHoraInicio { get; set; }
        public virtual DateTime DataHoraFim { get; set; }
        public virtual bool Ativo { get; set; }
    }
}