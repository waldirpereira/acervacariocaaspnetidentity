using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroCategoriasArtigos
{
    public class CadastroCategoriasArtigosMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<CategoriaArtigo, CategoriaArtigoViewModel>()
                .ReverseMap();
        }
    }
}