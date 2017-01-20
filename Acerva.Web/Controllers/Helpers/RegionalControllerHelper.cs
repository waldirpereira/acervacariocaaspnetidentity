using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using Acerva.Web.Models.CadastroRegionais;

namespace Acerva.Web.Controllers.Helpers
{
    public class RegionalControllerHelper
    {
        private readonly ICadastroRegionais _cadastroRegionais;

        public RegionalControllerHelper(ICadastroRegionais cadastroRegionais)
        {
            _cadastroRegionais = cadastroRegionais;
        }

        public IEnumerable<PartidaViewModel> PegaPartidasTerminadasNestaAtualizacao(
            RegionalViewModel regionalViewModel, Regional regional)
        {
            var partidasNaoTerminadasNoBd =
                regional.Rodadas.SelectMany(rodada => rodada.Partidas).Where(partida => !partida.Terminada);
            var partidasTerminadasNestaAtualizacao = from r in regionalViewModel.Rodadas
                                                     from p in r.Partidas
                                                     where p.Codigo > 0
                                                           && p.Terminada
                                                           && (partidasNaoTerminadasNoBd.Select(partida => partida.Codigo).Contains(p.Codigo))
                                                     select p;
            return partidasTerminadasNestaAtualizacao;
        }

        public void CalculaPontuacoesParaPartidasTerminadas(IEnumerable<PartidaViewModel> partidasViewModel)
        {
            partidasViewModel.ToList().ForEach(p => CalculaPontuacaoDaPartida(p));
        }

        public List<Palpite> CalculaPontuacaoDaPartida(PartidaViewModel partidaViewModel)
        {
            var palpitesDaPartida = _cadastroRegionais.PegaPalpitesDeUmaPartida(partidaViewModel.Codigo).ToList();

            palpitesDaPartida.ForEach(palpite =>
            {
                var pontuacao = 0;
                var partida = Mapper.Map<Partida>(partidaViewModel);
                var criteriosSatisfeitos = Criterio.CalculaCriterio(partida, palpite).ToList();
                var criterio = Criterio.CriterioNenhumAcerto;
                if (criteriosSatisfeitos.Any())
                {
                    var regras = criteriosSatisfeitos.Select(c => _cadastroRegionais.BuscaRegraDoCriterioParaAcerva(c, palpite.Participacao.Acerva)).ToList();
                    var regraDeMaiorPontuacao = regras.Where(r => r != null).OrderByDescending(r => r.Pontuacao).FirstOrDefault();
                    pontuacao = regraDeMaiorPontuacao?.Pontuacao ?? 0;
                    criterio = regraDeMaiorPontuacao?.Criterio ?? Criterio.CriterioNenhumAcerto;
                }

                palpite.Pontuacao = pontuacao;
                palpite.DataHoraPontuacao = DateTime.Now;
                palpite.Criterio = criterio;
            });

            return palpitesDaPartida;
        }
    }
}