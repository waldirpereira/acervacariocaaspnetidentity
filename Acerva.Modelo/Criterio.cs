using System;
using System.Collections.Generic;
using System.Linq;

namespace Acerva.Modelo
{
    public class Criterio : IEquatable<Criterio>
    {
        public virtual int Codigo { get; set; }
        public virtual int Ordem { get; set; }
        public virtual string Nome { get; set; }
        public virtual int PontuacaoPadrao { get; set; }

        public virtual Func<Partida, Palpite, bool> Satisfaz { get; set; }

        public static readonly Criterio CriterioPlacarCheio = new Criterio {Codigo = 1, Ordem = 1, Nome = "Acertar o placar em cheio", 
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                return (partida.PlacarMandante == palpite.PlacarMandante && partida.PlacarVisitante == palpite.PlacarVisitante);
            }
        };
        public static readonly Criterio CriterioVencedorESaldo = new Criterio {Codigo = 2, Ordem = 2, Nome = "Acertar a equipe vencedora e o saldo de gols da partida",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                var saldoPalpite = Math.Abs(palpiteMandante - palpiteVisitante);
                var saldoPartida = Math.Abs(partida.PlacarMandante.Value - partida.PlacarVisitante.Value);

                if (saldoPalpite == 0 || saldoPartida == 0)
                    return false;

                //apostou no mandante
                if (palpiteMandante > palpiteVisitante)
                {
                    return (partida.PlacarMandante.Value > partida.PlacarVisitante.Value && saldoPalpite == saldoPartida);
                }

                //apostou no visitante
                return (partida.PlacarMandante.Value < partida.PlacarVisitante.Value && saldoPalpite == saldoPartida);
            }
        };
        public static readonly Criterio CriterioVencedorEPlacarVencedor = new Criterio {Codigo = 3, Ordem = 3, Nome = "Acertar a equipe vencedora e o placar da equipe vencedora",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                var saldoPalpite = Math.Abs(palpiteMandante - palpiteVisitante);
                var saldoPartida = Math.Abs(partida.PlacarMandante.Value - partida.PlacarVisitante.Value);

                if (saldoPalpite == 0 || saldoPartida == 0)
                    return false;

                //apostou no mandante
                if (palpiteMandante > palpiteVisitante)
                {
                    return (partida.PlacarMandante.Value > partida.PlacarVisitante.Value && palpiteMandante == partida.PlacarMandante.Value);
                }

                //apostou no visitante
                return (partida.PlacarMandante.Value < partida.PlacarVisitante.Value && palpiteVisitante == partida.PlacarVisitante.Value);
            }
        };
        public static readonly Criterio CriterioVencedorEPlacarPerdedor = new Criterio {Codigo = 4, Ordem = 4, Nome = "Acertar a equipe vencedora e o placar da equipe perdedora",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                var saldoPalpite = Math.Abs(palpiteMandante - palpiteVisitante);
                var saldoPartida = Math.Abs(partida.PlacarMandante.Value - partida.PlacarVisitante.Value);

                if (saldoPalpite == 0 || saldoPartida == 0)
                    return false;

                //apostou no mandante
                if (palpiteMandante > palpiteVisitante)
                {
                    return (partida.PlacarMandante.Value > partida.PlacarVisitante.Value && palpiteVisitante == partida.PlacarVisitante.Value);
                }

                //apostou no visitante
                return (partida.PlacarMandante.Value < partida.PlacarVisitante.Value && palpiteMandante == partida.PlacarMandante.Value);
            }
        };
        public static readonly Criterio CriterioVencedor = new Criterio {Codigo = 5, Ordem = 5, Nome = "Acertar a equipe vencedora",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                var saldoPalpite = Math.Abs(palpiteMandante - palpiteVisitante);
                var saldoPartida = Math.Abs(partida.PlacarMandante.Value - partida.PlacarVisitante.Value);

                if (saldoPalpite == 0 || saldoPartida == 0)
                    return false;

                //apostou no mandante
                if (palpiteMandante > palpiteVisitante)
                {
                    return (partida.PlacarMandante.Value > partida.PlacarVisitante.Value);
                }

                //apostou no visitante
                return (partida.PlacarMandante.Value < partida.PlacarVisitante.Value);
            }
        };
        public static readonly Criterio CriterioEmpate = new Criterio {Codigo = 6, Ordem = 6, Nome = "Acertar o empate sem acertar o placar",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                var saldoPalpite = Math.Abs(palpiteMandante - palpiteVisitante);
                var saldoPartida = Math.Abs(partida.PlacarMandante.Value - partida.PlacarVisitante.Value);

                if (saldoPalpite != 0 || saldoPartida != 0)
                    return false;
                
                // foi empate e apostou empate
                return true;
            }
        };
        public static readonly Criterio CriterioPlacarDeUmaEquipe = new Criterio {Codigo = 7, Ordem = 7, Nome = "Acertar o placar de uma das equipes",
            Satisfaz = (partida, palpite) =>
            {
                if (SemPalpiteOuSemPlacarDaPartida(partida, palpite))
                    return false;

                var palpiteMandante = palpite.PlacarMandante.Value;
                var palpiteVisitante = palpite.PlacarVisitante.Value;
                
                return (palpiteMandante == partida.PlacarMandante.Value || palpiteVisitante == partida.PlacarVisitante.Value);
            }
        };
        public static readonly Criterio CriterioNenhumAcerto = new Criterio {Codigo = 999, Ordem = 999, Nome = "Nenhum acerto",
            Satisfaz = (partida, palpite) => !CriterioPlacarCheio.Satisfaz(partida, palpite)
                                             && !CriterioVencedorESaldo.Satisfaz(partida, palpite)
                                             && !CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpite)
                                             && !CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpite)
                                             && !CriterioVencedor.Satisfaz(partida, palpite)
                                             && !CriterioPlacarDeUmaEquipe.Satisfaz(partida, palpite)
                                             && !CriterioEmpate.Satisfaz(partida, palpite)
        };

        private static bool SemPalpiteOuSemPlacarDaPartida(Partida partida, Palpite palpite)
        {
            var palpiteMandante = palpite.PlacarMandante;
            var palpiteVisitante = palpite.PlacarVisitante;
            if (!palpiteMandante.HasValue || palpiteMandante < 0 || !palpiteVisitante.HasValue || palpiteVisitante < 0)
                return true;

            var placarMandante = partida.PlacarMandante;
            var placarVisitante = partida.PlacarVisitante;
            return (!placarMandante.HasValue || placarMandante < 0 || !placarVisitante.HasValue || placarVisitante < 0) ;
        }

        public static IEnumerable<Criterio> CalculaCriterio(Partida partida, Palpite palpite)
        {
            var criterios = new List<Criterio>
            {
                CriterioEmpate,
                CriterioNenhumAcerto,
                CriterioPlacarCheio,
                CriterioPlacarDeUmaEquipe,
                CriterioVencedor,
                CriterioVencedorEPlacarPerdedor,
                CriterioVencedorEPlacarVencedor,
                CriterioVencedorESaldo
            };

            return criterios.Where(c => c.Satisfaz(partida, palpite));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var objNovo = obj as Criterio;
            return objNovo != null && Equals(objNovo);
        }

        public virtual bool Equals(Criterio other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Codigo == Codigo;
        }

        public override int GetHashCode()
        {
            return Codigo;
        }
    }
}