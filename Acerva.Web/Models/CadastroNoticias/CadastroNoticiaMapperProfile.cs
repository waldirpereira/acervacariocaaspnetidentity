using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroNoticias
{
    public class CadastroNoticiaMapperProfile : Profile
    {
        public CadastroNoticiaMapperProfile()
        {
            CreateMap<Noticia, NoticiaViewModel>()
                .ReverseMap();
        }
    }
}