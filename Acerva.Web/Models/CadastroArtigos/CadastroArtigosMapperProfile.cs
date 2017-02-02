using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroArtigos
{
    public class CadastroArtigosMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Artigo, ArtigoViewModel>()
                .ReverseMap();
            CreateMap<CategoriaArtigo, CategoriaArtigoViewModel>()
                .ReverseMap();
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(d => d.NomeRegional, o => o.ResolveUsing(s => s.Regional.Nome))
                .ReverseMap();
        }
    }
}