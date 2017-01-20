using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Acerva.Infra.Web;
using Acerva.Web.AttributeAdapters;
using Ninject.Modules;
using Ninject.Web.Mvc.FilterBindingSyntax;

namespace Acerva.Web.Ninject
{
    [ExcludeFromCodeCoverage]
    public class HttpFiltersNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.BindFilter<HandleErrorFazendoLogAttribute>(FilterScope.Global, 1);

            this.BindFilter<TransacaoFilter>(FilterScope.Action, 0)
                .WhenActionMethodHas<TransacaoAttribute>();
        }
    }
}