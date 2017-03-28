using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CartilhaBoasVindas
{
    public class CartilhaBoasVindasMapperProfile : Profile
    {
        public CartilhaBoasVindasMapperProfile()
        {
            CreateMap<Artigo, ArtigoViewModel>()
                .ReverseMap();

            CreateMap<AnexoArtigo, AnexoArtigoViewModel>()
                .ReverseMap();
        }
    }
}