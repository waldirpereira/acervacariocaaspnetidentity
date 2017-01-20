using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroAcervas
{
    public class CadastroAcervaMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Modelo.Acerva, AcervaViewModel>()
                .ReverseMap()
                .BeforeMap((s, d) =>
                {
                    if (s.Participacoes == null)
                        s.Participacoes = new List<ParticipacaoViewModel>();
                    if (s.Regras == null)
                        s.Regras = new List<RegraViewModel>();
                })
                .ForMember(d => d.Regional, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.UsuarioResponsavel, o => o.DoNotUseDestinationValue())
                .ForMember(s => s.Regras, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNasRegras)
                .ForMember(s => s.Participacoes, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNasParticipacoes);

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();

            CreateMap<Regra, RegraViewModel>()
                .ReverseMap();

            CreateMap<Criterio, CriterioViewModel>()
                .ReverseMap();

            CreateMap<Participacao, ParticipacaoViewModel>()
                .ForMember(d => d.CodigoAcerva, o => o.MapFrom(s => s.Acerva.Codigo))
                .ReverseMap()
                .ForMember(d => d.Usuario, o => o.UseDestinationValue())
                .ForMember(d => d.DataHoraInclusao, o =>
                {
                    o.Condition(c => c.DataHoraInclusao == null);
                    o.MapFrom(s => DateTime.Now);
                });

            CreateMap<IdentityUser, IdentityUserViewModel>()
                .ReverseMap()
                .ForMember(d => d.Id, o => o.NullSubstitute("novo"))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email));
        }
        

        private static void ProcessaAlteracoesNasRegras(AcervaViewModel source, Modelo.Acerva dest)
        {
            (from regra in dest.Regras
             where source.Regras.All(regraViewModel => regraViewModel.Codigo != regra.Codigo)
             select regra)
                .ToList()
                .ForEach(regra => dest.Regras.Remove(regra));

            foreach (var regraViewModel in source.Regras)
            {
                var regraNoAcerva = dest.Regras.FirstOrDefault(regra => regra.Codigo == regraViewModel.Codigo);
                if (regraNoAcerva != null)
                {
                    Mapper.Map(regraViewModel, regraNoAcerva);
                    continue;
                }

                regraNoAcerva = Mapper.Map<Regra>(regraViewModel);
                regraNoAcerva.Codigo = 0;
                regraNoAcerva.Acerva = dest;
                dest.Regras.Add(regraNoAcerva);
            }
        }

        private static void ProcessaAlteracoesNasParticipacoes(AcervaViewModel source, Modelo.Acerva dest)
        {
            (from participacao in dest.Participacoes
             where source.Participacoes.All(participacaoViewModel => participacaoViewModel.Codigo != participacao.Codigo)
             select participacao)
                .ToList()
                .ForEach(participacao => dest.Participacoes.Remove(participacao));

            foreach (var participacaoViewModel in source.Participacoes)
            {
                var participacaoNoAcerva = dest.Participacoes.FirstOrDefault(participacao => participacao.Codigo == participacaoViewModel.Codigo);
                if (participacaoNoAcerva != null)
                {
                    Mapper.Map(participacaoViewModel, participacaoNoAcerva);
                    continue;
                }

                participacaoNoAcerva = Mapper.Map<Participacao>(participacaoViewModel);
                participacaoNoAcerva.Codigo = 0;
                participacaoNoAcerva.Acerva = dest;
                if (participacaoNoAcerva.Usuario.Id == "novo")
                    participacaoNoAcerva.Usuario.Id = Guid.NewGuid().ToString();
                dest.Participacoes.Add(participacaoNoAcerva);
            }
        }
    }
}