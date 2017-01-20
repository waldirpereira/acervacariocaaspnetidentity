using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Models.CadastroRegionais;
using log4net;

namespace Acerva.Web.Controllers
{
    public class ImportacaoController : Controller
    {
        private readonly ICadastroRegionais _cadastroRegionais;
        private readonly RegionalControllerHelper _helper;

        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ImportacaoController(ICadastroRegionais cadastroRegionais, RegionalControllerHelper helper)
        {
            _cadastroRegionais = cadastroRegionais;
            _helper = helper;
        }

        public IEnumerable<Partida> BuscaAtualizacoesPartidasDaRodada(Rodada rodada)
        {
            const string url = "http://www.tabeladobrasileirao.net/";
            const string tablePattern = "<table.*?data-module=\"table-rounds\".*?>(.*?)</table>";
            var trPattern = "<tr  class=\"table-row\" data-round=" + rodada.Codigo + ".*?>(.*?)</tr>";
            const string tdPattern = "<td.*?>(.*?)</td>";
            const string divHomePattern = "<div.*?class=\".*?game-club--principal.*?\".*?title=\"(.*?)\".*?>.*?";
            const string divVisitorPattern = "<div.*?class=\".*?game-club--visitor.*?\".*?title=\"(.*?)\".*?>.*?";
            const string divGameScoreBoardValuesPattern = "<div class=\"game-scoreboard-values\">(<div.*?>.*?</div>)</div>";
            const string divGameScoreBoardInputPattern = "<div class=\"game-scoreboard-input\">(.*?)</div>";

            /*
            <table class="table" data-current-round="15" data-max-rounds="38" data-module="table-rounds" id="mod-table-rounds-1">
              ...
              <tr class="table-row" data-round="15" data-group="null">
                <td class="match">                                                                                                             <!-- tdMatchIndex: 0 -->
                  <div class="game" data-group="0">
                    <div class="game-round round" data-round="15" style="display: none;">15</div>
                    <div class="game-scoreboard">
                      <div class="game-club game-club--principal home home_name" data-home="18" data-home-name="Santos" title="Santos">
                        SAN <i class="shield shield-santos"></i>
                      </div>
                      <div class="game-scoreboard-values">
                        <div class="game-scoreboard-input">3</div>                                                                                    <!-- homeScoreCellIndex: 0 -->
                        <div class="game-scoreboard-input">x</div>
                        <div class="game-scoreboard-input">1</div>                                                                                    <!-- visitorScoreCellIndex: 2 -->
                      </div>
                      <div class="game-club game-club--visitor visitor visitor_name" data-visitor="34" data-visitor-name="Santos" title="Ponte Preta">
                        <i class="shield shield-ponte-preta"></i> PON
                      </div>
                    </div>
                  </div>
                </td>
                <td class="date">16/07</td>                                                                                                    <!-- tdDataIndex: 1 -->
                <td class="day">Sab</td>
                <td class="hour">18:30</td>                                                                                                    <!-- tdHoraIndex: 3 -->
                <td class="stadium">Vila Belmiro</td>
                <td class="local">Santos - SP</td>
                <td class="tv"><i class="icon-tv icon-tv-pfc"></i>
                </td>
              </tr>
            </table>
            */

            const int tdMatchIndex = 0;
            const int tdDataIndex = 1;
            const int tdHoraIndex = 3;

            const int homeScoreCellIndex = 0;
            const int visitorScoreCellIndex = 2;
            var partidasAtualizadas = new List<Partida>();

            using (var client = new WebClient())
            {
                var proxy = WebRequest.GetSystemWebProxy();
                proxy.Credentials = CredentialCache.DefaultCredentials;
                client.UseDefaultCredentials = true;
                client.Proxy = proxy;
                client.Encoding = System.Text.Encoding.UTF8;

                try
                {
                    var content = client.DownloadString(url);
                    var tableContents = GetContents(content, tablePattern);
                    var equipes = _cadastroRegionais.BuscaEquipesDoRegional(rodada.Regional.Codigo).ToList();

                    var rows = GetContents(tableContents.FirstOrDefault(), trPattern);
                    foreach (var row in rows)
                    {
                        var cells = GetContents(row, tdPattern);

                        var dataContent = GetContentBetweenTags(cells[tdDataIndex], tdPattern);
                        var arrDataContent = dataContent.Split('/');
                        var dataHoraPartida = new DateTime(DateTime.Now.Year, Convert.ToInt32(arrDataContent[1]), Convert.ToInt32(arrDataContent[0]));

                        var horaContent = GetContentBetweenTags(cells[tdHoraIndex], tdPattern);
                        var arrHoraContent = horaContent.Split(':');
                        dataHoraPartida = dataHoraPartida.AddHours(Convert.ToInt32(arrHoraContent[0])).AddMinutes(Convert.ToInt32(arrHoraContent[1]));

                        var match = cells[tdMatchIndex];

                        var equipeMandante = equipes.FirstOrDefault(e => e.Nome == GetContentBetweenTags(match, divHomePattern));
                        var equipeVisitante = equipes.FirstOrDefault(e => e.Nome == GetContentBetweenTags(match, divVisitorPattern));

                        if (equipeMandante == null || equipeVisitante == null)
                            continue;

                        var scoreContainer = GetContentBetweenTags(match, divGameScoreBoardValuesPattern);
                        var scoreCells = GetContents(scoreContainer, divGameScoreBoardInputPattern);

                        if (scoreCells.Count < 3)
                            continue;

                        var placarMandante = ToNullableInt(GetContentBetweenTags(scoreCells[homeScoreCellIndex], divGameScoreBoardInputPattern));
                        var placarVisitante = ToNullableInt(GetContentBetweenTags(scoreCells[visitorScoreCellIndex], divGameScoreBoardInputPattern));

                        var partida = rodada.Partidas.FirstOrDefault(p => p.EquipeMandante == equipeMandante && p.EquipeVisitante == equipeVisitante && p.Terminada == false);

                        if (partida == null)
                            continue;

                        if (partida.DataHora != dataHoraPartida
                            || partida.PlacarMandante != placarMandante
                            || partida.PlacarVisitante != placarVisitante)
                        {
                            partidasAtualizadas.Add(partida);
                        }

                        partida.DataHora = dataHoraPartida;

                        if (!placarMandante.HasValue || !placarVisitante.HasValue || dataHoraPartida.AddMinutes(150) > DateTime.Now)
                            continue;

                        partida.PlacarMandante = placarMandante;
                        partida.PlacarVisitante = placarVisitante;
                        partida.Terminada = true;
                    }
                }
                catch (WebException ex)
                {
                    Log.InfoFormat("Erro ao tentar baixar conteúdo da url '{0}': {1}. ", url, ex.Message);
                }
            }

            return partidasAtualizadas;
        }

        private static List<string> GetContents(string input, string pattern)
        {
            var matches = Regex.Matches(input, pattern, RegexOptions.Singleline);
            return (from Match match in matches select match.Value).ToList();
        }
        private static string GetContentBetweenTags(string input, string pattern)
        {
            var regex = new Regex(pattern);
            var v = regex.Match(input);
            return v.Groups[1].ToString();
        }

        private static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
        
        public void AtualizaPartidasDasProximasRodadas()
        {
            var regionais = _cadastroRegionais.BuscaTodos().Where(c => c.Codigo == 1).ToList(); //Apenas Brasileiro 2016 por enquanto!!!
            using (var transacao = _cadastroRegionais.BeginTransaction())
            { 
                regionais.ForEach(c =>
                {
                    var rodadas = c.Rodadas
                        .Where(r => r.Partidas.Any(p => !p.Terminada))
                        .OrderBy(r => r.Ordem)
                        .Take(5)
                        .ToList();

                    var partidasAtualizadas = new List<Partida>();
                    rodadas.ForEach(r =>
                    {
                        partidasAtualizadas.AddRange(BuscaAtualizacoesPartidasDaRodada(r));

                        Log.InfoFormat("{0} partidas atualizadas da rodada {1} da regional {2}", partidasAtualizadas, r.Nome, c.Nome);
                    });

                    if (partidasAtualizadas.Count <= 0)
                        return;

                    var partidasTerminadasNestaAtualizacao = partidasAtualizadas.Where(p => p.Terminada).ToList();
                    partidasTerminadasNestaAtualizacao.ForEach(p =>
                    {
                        var palpitesAlterados =
                            _helper.CalculaPontuacaoDaPartida(Mapper.Map<PartidaViewModel>(p)).ToList();
                        palpitesAlterados.ForEach(_cadastroRegionais.SalvaPalpite);
                    });
                    _cadastroRegionais.Salva(c);
                });

                _cadastroRegionais.CommitTransaction(transacao);
            }
        }
    }
}