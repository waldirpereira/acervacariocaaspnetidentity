using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Acerva.Web.Startup))]
namespace Acerva.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
