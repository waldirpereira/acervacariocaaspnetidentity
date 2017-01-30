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

            CreateMap<Usuario, UsuarioRegistroViewModel>()
                .ReverseMap()
                .ForMember(d => d.UserName, o => o.ResolveUsing(s => s.Email))
                .ForMember(d => d.Regional, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.UsuarioIndicacao, o => o.DoNotUseDestinationValue());

            CreateMap<Usuario, UsuarioIndicacaoViewModel>()
                .ForMember(d => d.NomeRegional, o => o.ResolveUsing(s => s.Regional.Nome))
                .ReverseMap();

            //CreateMap<Base64UploadFile, Base64UploadFileViewModel>()
            //    .ReverseMap();

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }
    }
}