using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroCategoriasArtigos
{
    public class CadastroCategoriasArtigosMapperProfile : Profile
    {
        public CadastroCategoriasArtigosMapperProfile()
        {
            CreateMap<CategoriaArtigo, CategoriaArtigoViewModel>()
                .ReverseMap();
        }
    }
}