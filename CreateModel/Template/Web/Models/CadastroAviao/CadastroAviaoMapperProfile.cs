using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroAvioes
{
    public class CadastroAviaoMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Aviao, AviaoViewModel>()
                .ReverseMap();
        }
    }
}