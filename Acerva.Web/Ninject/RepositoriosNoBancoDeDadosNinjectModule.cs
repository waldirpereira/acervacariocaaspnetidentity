using Acerva.Infra.Repositorios;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Acerva.Web.Ninject
{
    public class RepositoriosNoBancoDeDadosNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(ICadastroRegionais))
                .To(typeof(CadastroRegionais))
                .InRequestScope();

            Bind(typeof(ICadastroNoticias))
                .To(typeof(CadastroNoticias))
                .InRequestScope();

            Bind(typeof(ICadastroUsuarios))
                .To(typeof(CadastroUsuarios))
                .InRequestScope();

            Bind(typeof(ICadastroArtigos))
                .To(typeof(CadastroArtigos))
                .InRequestScope();
        }

    }
}