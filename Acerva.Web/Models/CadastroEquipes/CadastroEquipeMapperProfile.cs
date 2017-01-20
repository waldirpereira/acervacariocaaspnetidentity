using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroEquipes
{
    public class CadastroEquipeMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Equipe, EquipeViewModel>()
                .ReverseMap();
        }
    }
}