using System.Web.Mvc;
using Acerva.Web.Controllers;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
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

            var configuration = new NHibernate.Cfg.Configuration();
            var connectionString = configuration.Properties[NHibernate.Cfg.Environment.ConnectionString];
            GlobalConfiguration.Configuration.UseMySqlStorage(connectionString);
            var options = new DashboardOptions
            {
                AuthorizationFilters = new[]
                {
                    new AuthorizationFilter { Roles = "ADMIN" }
                }
            };
            app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireServer();

            //var controller = DependencyResolver.Current.GetService<ImportacaoController>();
            //RecurringJob.AddOrUpdate("AtualizaPartidasDasProximasRodadas", () => controller.AtualizaPartidasDasProximasRodadas(), Cron.MinuteInterval(30));
        }
    }
}
