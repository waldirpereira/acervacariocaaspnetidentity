using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class CadastroUsuariosMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<IdentityUser, UserViewModel>()
                .ReverseMap();
        }
    }
}