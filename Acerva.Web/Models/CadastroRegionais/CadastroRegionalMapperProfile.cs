using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroRegionais
{
    public class CadastroRegionalMapperProfile : Profile
    {
        public CadastroRegionalMapperProfile()
        {
            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }
    }
}