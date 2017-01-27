using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class CadastroUsuariosMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Usuario, UsuarioViewModel>()
                .ReverseMap()
                .ForMember(d => d.UserName, o => o.ResolveUsing(s => s.Email))
                .ForMember(d => d.Regional, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.UsuarioIndicacao, o => o.DoNotUseDestinationValue());

            CreateMap<Usuario, UsuarioIndicacaoViewModel>()
                .ForMember(d => d.NomeRegional, o => o.ResolveUsing(s => s.UsuarioIndicacao != null ? s.UsuarioIndicacao.Regional.Nome : null))
                .ReverseMap();

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }
    }
}