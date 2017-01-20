using System;
using System.Collections.Generic;
using System.Linq;

namespace Acerva.Modelo
{
    public class Participacao
    {
        public virtual int Codigo { get; set; }
        public virtual Acerva Acerva { get; set; }
        public virtual IdentityUser Usuario { get; set; }
        public virtual int PontuacaoInicial { get; set; }
        public virtual DateTime? DataHoraInclusao { get; set; }

        private int? _pontuacao;
        public virtual int? PontuacaoAtual
        {
            get { return _pontuacao ?? Palpites.Sum(p => p.Pontuacao) + PontuacaoInicial; }
            set { _pontuacao = value; }
        }

        public virtual int Posicao { get; set; }
        
        public virtual int PosicaoRodadaAnterior { get; set; }


        private ICollection<Palpite> _palpites = new List<Palpite>();
        public virtual ICollection<Palpite> Palpites
        {
            get { return _palpites; }
            set { _palpites = value; }
        }

        public virtual int PosicaoInicial
        {
            get
            {
                var participantesOrdenadosPelaPontuacaoInicial =
                    Acerva.Participacoes.OrderBy(p => p.DataHoraInclusao).ThenByDescending(p => p.PontuacaoInicial).ToList();
                return participantesOrdenadosPelaPontuacaoInicial.IndexOf(this) + 1;
            }
        }
    }
}