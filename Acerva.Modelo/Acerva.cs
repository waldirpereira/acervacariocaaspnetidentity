using System.Collections.Generic;
using System.Linq;

namespace Acerva.Modelo
{
    public class Acerva
    {
        public const int TamanhoMaximoNome = 256;

        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual Regional Regional { get; set; }
        public virtual IdentityUser UsuarioResponsavel { get; set; }
        public virtual bool Ativo { get; set; }

        private ICollection<Participacao> _participacoes = new List<Participacao>();
        public virtual ICollection<Participacao> Participacoes
        {
            get { return _participacoes; }
            set { _participacoes = value; }
        }

        private ICollection<Regra> _regras = new List<Regra>();
        public virtual ICollection<Regra> Regras
        {
            get { return _regras; }
            set { _regras = value; }
        }

        public virtual void CalculaPontuacaoEPosicaoDasParticipacoes(Rodada rodada)
        {
            var participacoesOrdenadasRodadaAnterior = CalculaRankingAcumuladoAteRodada(rodada.Ordem - 1);
            for (var i = 0; i < participacoesOrdenadasRodadaAnterior.Count; i++)
            {
                participacoesOrdenadasRodadaAnterior[i].PosicaoRodadaAnterior = i + 1;
            }

            var participacoesOrdenadas = CalculaRankingAcumuladoAteRodada(rodada.Ordem);
            for (var i = 0; i < participacoesOrdenadas.Count; i++)
            {
                participacoesOrdenadas[i].Posicao = i+1;
                participacoesOrdenadas[i].PontuacaoAtual = Participacoes
                    .Where(p => p.Codigo == participacoesOrdenadas[i].Codigo)
                    .Sum(p => p.PontuacaoInicial + p.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= rodada.Ordem).Sum(palpite => palpite.Pontuacao));
            }
        }

        private List<Participacao> CalculaRankingAcumuladoAteRodada(int ordemRodada)
        {
            var participacoesOrdenadas = Participacoes
                .OrderByDescending(p7 => p7.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioPlacarDeUmaEquipe))
                .ThenByDescending(p6 => p6.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioVencedor))
                .ThenByDescending(p5 => p5.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioVencedorEPlacarPerdedor))
                .ThenByDescending(p4 => p4.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioVencedorEPlacarVencedor))
                .ThenByDescending(p3 => p3.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioVencedorESaldo))
                .ThenByDescending(p2 => p2.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Count(p => p.Criterio == Criterio.CriterioPlacarCheio))
                .ThenByDescending(p => p.PontuacaoInicial + p.Palpites.Where(palpite => palpite.Partida.Rodada.Ordem <= ordemRodada).Sum(palpite => palpite.Pontuacao))
                .ToList();
            return participacoesOrdenadas;
        }
    }
}
