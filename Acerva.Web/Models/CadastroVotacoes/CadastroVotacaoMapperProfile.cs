using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroVotacoes
{
    public class CadastroVotacaoMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Votacao, VotacaoViewModel>()
                .ReverseMap();
        }
    }
}