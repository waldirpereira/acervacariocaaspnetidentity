using System;
using System.Linq;
using AutoMapper;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class CadastroUsuariosMapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(d => d.NomesPapeis, o => o.ResolveUsing(s => s.Papeis.Any() ? s.Papeis.Select(p => p.Name).Aggregate((x, y) => x + ", " + y) : string.Empty))
                .ReverseMap()
                .ForMember(d => d.UserName, o => o.ResolveUsing(s => s.Email))
                .ForMember(d => d.Regional, o => o.DoNotUseDestinationValue())
                .ForMember(d => d.UsuarioIndicacao, o => o.DoNotUseDestinationValue())
                .ForMember(s => s.Papeis, o => o.Ignore())
                .AfterMap(ProcessaAlteracoesNosPapeis);

            CreateMap<Usuario, UsuarioIndicacaoViewModel>()
                .ForMember(d => d.NomeRegional, o => o.ResolveUsing(s => s.UsuarioIndicacao != null ? s.UsuarioIndicacao.Regional.Nome : null))
                .ReverseMap();

            CreateMap<Papel, PapelViewModel>()
                .ReverseMap();

            CreateMap<Regional, RegionalViewModel>()
                .ReverseMap();
        }


        private static void ProcessaAlteracoesNosPapeis(UsuarioViewModel s, Usuario d)
        {
            (from a in d.Papeis
             where s.Papeis.All(objVm => objVm.Id != a.Id)
             select a)
                .ToList()
                .ForEach(objFilho => d.Papeis.Remove(objFilho));

            foreach (var objVm in s.Papeis)
            {
                var obj = d.Papeis.FirstOrDefault(objFilho => objFilho.Id == objVm.Id);
                if (obj != null)
                {
                    Mapper.Map(objVm, obj);
                    continue;
                }

                obj = Mapper.Map<Papel>(objVm);
                d.Papeis.Add(obj);
            }
        }
    }
}