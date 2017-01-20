using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroRegionais
{
    public class CadastroRegionalMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Partida, PartidaViewModel>()
                .ReverseMap()
                .ForMember(d => d.Rodada, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.EquipeMandante, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.EquipeVisitante, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.DataHora, o => o.ResolveUsing(s =>
                {
                    if (!s.Data.HasValue || s.Horario == null)
                        return null;
                    
                    return s.Data.Value.Date.AddHours(s.Horario.Hour).AddMinutes(s.Horario.Minute);
                }));

            CreateMap<Rodada, RodadaViewModel>()
                .ReverseMap()
                .BeforeMap((s, d) =>
                {
                    if (s.Partidas == null)
                        s.Partidas = new List<PartidaViewModel>();
                })
                .ForMember(s => s.Regional, o => o.DoNotUseDestinationValue())
                .ForMember(s => s.Partidas, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNasPartidas);

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap()
                .BeforeMap((s, d) =>
                {
                    if (s.Rodadas == null)
                        s.Rodadas = new List<RodadaViewModel>();
                })
                .BeforeMap((s, d) =>
                {
                    if (s.Equipes == null)
                        s.Equipes = new List<EquipeViewModel>();
                })
                .ForMember(d => d.Equipes, o => o.DoNotUseDestinationValue())
                .ForMember(s => s.Rodadas, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNasRodadas);

            CreateMap<Equipe, EquipeViewModel>()
                .ReverseMap();
        }

        private static void ProcessaAlteracoesNasPartidas(RodadaViewModel source, Rodada dest)
        {
            (from partida in dest.Partidas
             where source.Partidas.All(partidaViewModel => partidaViewModel.Codigo != partida.Codigo)
             select partida)
                .ToList()
                .ForEach(partida => dest.Partidas.Remove(partida));

            foreach (var partidaViewModel in source.Partidas)
            {
                var partidaNaRodada = dest.Partidas.FirstOrDefault(partida => partida.Codigo == partidaViewModel.Codigo);
                if (partidaNaRodada != null)
                {
                    Mapper.Map(partidaViewModel, partidaNaRodada);
                    continue;
                }

                partidaNaRodada = Mapper.Map<Partida>(partidaViewModel);
                partidaNaRodada.Codigo = 0;
                partidaNaRodada.Rodada = dest;
                dest.Partidas.Add(partidaNaRodada);
            }
        }

        private static void ProcessaAlteracoesNasRodadas(RegionalViewModel source, Regional dest)
        {
            (from rodada in dest.Rodadas
             where source.Rodadas.All(rodadaViewModel => rodadaViewModel.Codigo != rodada.Codigo)
             select rodada)
                .ToList()
                .ForEach(rodada => dest.Rodadas.Remove(rodada));

            foreach (var rodadaViewModel in source.Rodadas)
            {
                var rodadaNoRegional = dest.Rodadas.FirstOrDefault(rodada => rodada.Codigo == rodadaViewModel.Codigo);
                if (rodadaNoRegional != null)
                {
                    Mapper.Map(rodadaViewModel, rodadaNoRegional);
                    continue;
                }

                rodadaNoRegional = Mapper.Map<Rodada>(rodadaViewModel);
                rodadaNoRegional.Codigo = 0;
                rodadaNoRegional.Regional = dest;
                dest.Rodadas.Add(rodadaNoRegional);
            }
        }
    }
}