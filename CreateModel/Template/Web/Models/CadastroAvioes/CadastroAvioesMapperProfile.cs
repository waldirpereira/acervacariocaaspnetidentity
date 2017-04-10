using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroAvioes
{
    public class CadastroAvioesMapperProfile : Profile
    {
        public CadastroAvioesMapperProfile()
        {
            CreateMap<Aviao, AviaoViewModel>()
                .ReverseMap();
        }
    }
}