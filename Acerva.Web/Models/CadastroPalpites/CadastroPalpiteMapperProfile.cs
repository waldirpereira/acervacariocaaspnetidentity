using System.Linq;
using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroPalpites
{
    public class CadastroPalpiteMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Modelo.Acerva, AcervaViewModel>()
                .ReverseMap();
            
            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();

            CreateMap<IdentityUser, IdentityUserViewModel>()
                .ReverseMap();

            CreateMap<Equipe, EquipeViewModel>()
                .ReverseMap();

            CreateMap<Partida, PartidaViewModel>()
                .ForMember(d => d.CodigoRodada, o => o.ResolveUsing(s => s.Rodada.Codigo))
                .ForMember(d => d.NomeRodada, o => o.ResolveUsing(s => s.Rodada.Nome))
                .ForMember(d => d.OrdemRodada, o => o.ResolveUsing(s => s.Rodada.Ordem))
                .ReverseMap()
                .ForMember(d => d.DataHora, o => o.ResolveUsing(s =>
                {
                    if (!s.Data.HasValue || s.Horario == null)
                        return null;

                    return s.Data.Value.Date.AddHours(s.Horario.Hour).AddMinutes(s.Horario.Minute);
                }));

            CreateMap<Palpite, PalpiteViewModel>()
                .ReverseMap()
                .ForMember(d => d.Partida, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.Participacao, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.Criterio, o => o.DoNotUseDestinationValue());

            CreateMap<Criterio, CriterioViewModel>()
                .ReverseMap();

            CreateMap<Participacao, ParticipacaoViewModel>()
                .ForMember(d => d.AcervaAtivo, o => o.ResolveUsing(s => s.Acerva.Ativo))
                .ForMember(d => d.CodigoAcerva, o => o.ResolveUsing(s => s.Acerva.Codigo))
                .ForMember(d => d.NomeAcerva, o => o.ResolveUsing(s => s.Acerva.Nome))
                .ForMember(d => d.CodigoRegional, o => o.ResolveUsing(s => s.Acerva.Regional.Codigo))
                .ForMember(d => d.NomeRegional, o => o.ResolveUsing(s => s.Acerva.Regional.Nome))
                .ReverseMap()
                .ForMember(d => d.Usuario, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.Palpites, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNosPalpites);
        }

        private static void ProcessaAlteracoesNosPalpites(ParticipacaoViewModel source, Participacao dest)
        {
            (from palpite in dest.Palpites
             where source.Palpites.All(palpiteViewModel => palpiteViewModel.Codigo != palpite.Codigo)
             select palpite)
                .ToList()
                .ForEach(palpite => dest.Palpites.Remove(palpite));

            foreach (var palpiteViewModel in source.Palpites)
            {
                var palpiteNaParticipacao = dest.Palpites.FirstOrDefault(palpite => palpite.Codigo == palpiteViewModel.Codigo);
                if (palpiteNaParticipacao != null)
                {
                    Mapper.Map(palpiteViewModel, palpiteNaParticipacao);
                    continue;
                }

                palpiteNaParticipacao = Mapper.Map<Palpite>(palpiteViewModel);
                palpiteNaParticipacao.Codigo = 0;
                palpiteNaParticipacao.Participacao = dest;
                dest.Palpites.Add(palpiteNaParticipacao);
            }
        }
    }
}