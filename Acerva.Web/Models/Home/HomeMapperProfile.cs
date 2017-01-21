using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.Home
{
    public class HomeMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Noticia, NoticiaViewModel>()
                .ReverseMap();
        }
    }
}