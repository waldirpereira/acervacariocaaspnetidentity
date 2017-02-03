using System.Linq;
using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.Referencia
{
    public class ReferenciaMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Artigo, ArtigoViewModel>()
                .ForMember(d => d.NomeUsuario, o => o.ResolveUsing(s => s.Usuario.Name))
                .ReverseMap();

            CreateMap<Artigo, ArtigoListaViewModel>()
                .ForMember(d => d.NomeUsuario, o => o.ResolveUsing(s => s.Usuario.Name))
                .ReverseMap();

            CreateMap<AnexoArtigo, AnexoArtigoViewModel>()
                .ReverseMap();

            CreateMap<CategoriaArtigo, CategoriaArtigoViewModel>()
                .ForMember(d => d.QtdArtigos, o => o.ResolveUsing(s => s.Artigos.Count()))
                .ReverseMap();
        }
    }
}