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
                .ForMember(d => d.Regional, o => o.DoNotUseDestinationValue());

            CreateMap<Usuario, UsuarioIndicacaoViewModel>()
                .ReverseMap();

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }
    }
}