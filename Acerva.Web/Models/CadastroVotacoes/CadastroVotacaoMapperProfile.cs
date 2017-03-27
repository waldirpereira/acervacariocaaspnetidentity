using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroVotacoes
{
    public class CadastroVotacaoMapperProfile : Profile
    {
        public CadastroVotacaoMapperProfile()
        {
            CreateMap<Votacao, VotacaoViewModel>()
                .ReverseMap();
        }
    }
}