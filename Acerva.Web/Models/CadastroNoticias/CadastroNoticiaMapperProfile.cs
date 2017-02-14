using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroNoticias
{
    public class CadastroNoticiaMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Noticia, NoticiaViewModel>()
                .ReverseMap();
        }
    }
}