using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroRegionais
{
    public class CadastroRegionalMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }
    }
}