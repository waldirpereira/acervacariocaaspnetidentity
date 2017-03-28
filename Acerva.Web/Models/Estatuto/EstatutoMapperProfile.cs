using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.Estatuto
{
    public class EstatutoMapperProfile : Profile
    {
        public EstatutoMapperProfile()
        {
            CreateMap<Artigo, ArtigoViewModel>()
                .ReverseMap();

            CreateMap<AnexoArtigo, AnexoArtigoViewModel>()
                .ReverseMap();
        }
    }
}