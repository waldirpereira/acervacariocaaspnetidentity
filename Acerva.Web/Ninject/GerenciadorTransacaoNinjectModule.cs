using System.Diagnostics.CodeAnalysis;
using Acerva.Infra.GerenciadorTransacao;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Acerva.Web.Ninject
{
    [ExcludeFromCodeCoverage]
    public class GerenciadorTransacaoNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IGerenciadorTransacao>()
                .To<GerenciadorTransacao>()
                .InRequestScope();
        }

    }
}