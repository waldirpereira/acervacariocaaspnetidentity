using Acerva.Infra.Repositorios;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Acerva.Web.Ninject
{
    public class RepositoriosNoBancoDeDadosNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(ICadastroEquipes))
                .To(typeof(CadastroEquipes))
                .InRequestScope();

            Bind(typeof(ICadastroRegionais))
                .To(typeof(CadastroRegionais))
                .InRequestScope();

            Bind(typeof(ICadastroRodadas))
                .To(typeof(CadastroRodadas))
                .InRequestScope();

            Bind(typeof(ICadastroPartidas))
                .To(typeof(CadastroPartidas))
                .InRequestScope();

            Bind(typeof(ICadastroAcervas))
                .To(typeof(CadastroAcervas))
                .InRequestScope();

            Bind(typeof(ICadastroUsuarios))
                .To(typeof(CadastroUsuarios))
                .InRequestScope();

            Bind(typeof(ICadastroParticipacoes))
                .To(typeof(CadastroParticipacoes))
                .InRequestScope();
        }

    }
}