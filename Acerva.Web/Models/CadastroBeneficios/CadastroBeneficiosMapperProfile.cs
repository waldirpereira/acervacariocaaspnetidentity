using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroBeneficios
{
    public class CadastroBeneficiosMapperProfile : Profile
    {
        public CadastroBeneficiosMapperProfile()
        {
            CreateMap<Beneficio, BeneficioViewModel>()
                .ReverseMap();
        }
    }
}