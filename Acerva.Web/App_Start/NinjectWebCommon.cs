using System.Reflection;
using System.Security.Principal;
using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using Acerva.Modelo.Mapeamento;
using Acerva.Modelo.Validadores;
using Acerva.Web.App_Start;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Ninject;
using FluentNHibernate.Cfg;
using FluentValidation;
using Hangfire;
using log4net;
using NHibernate;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]

namespace Acerva.Web.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    public static class NinjectWebCommon
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(
                new RepositoriosNoBancoDeDadosNinjectModule(),
                new HttpFiltersNinjectModule(),
                new GerenciadorTransacaoNinjectModule()
            );
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterServices(IKernel kernel)
        {
            Log.Info("Configurando Injeção de Dependências");

            kernel.Bind<ISession>()
                .ToMethod(m => MvcApplication.NewSession)
                .InRequestScope()
                .OnDeactivation(s => s.Dispose());

            kernel.Bind<ISessionFactory>()
                .ToMethod(ctx =>
                {
                    return Fluently.Configure()
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<HistoricoStatusUsuarioClassMap>())
                        .ExposeConfiguration(config => { })
                        .BuildSessionFactory();
                })
                .WhenInjectedInto(typeof(ICadastroHistoricoStatusUsuarios))
                .InRequestScope()
                .OnDeactivation(s => s.Dispose());

            kernel.Bind<IPrincipal>()
                .ToMethod(context => HttpContext.Current != null ? HttpContext.Current.User : null)
                .InRequestScope();

            AssemblyScanner.FindValidatorsInAssembly(Assembly.GetAssembly(typeof(RegionalValidator)))
                .ForEach(match => kernel.Bind(match.InterfaceType)
                .To(match.ValidatorType));

            kernel.Bind<UsuarioControllerHelper>()
                .ToSelf();
        }
    }
}
