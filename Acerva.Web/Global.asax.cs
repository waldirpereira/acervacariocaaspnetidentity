using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Acerva.Infra.Eventos;
using AutoMapper;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Modelo.Mapeamento;
using Acerva.Web.AttributeAdapters;
using Acerva.Web.Models.CadastroArtigos;
using Acerva.Web.Models.CadastroRegionais;
using Acerva.Web.Models.CadastroUsuarios;
using Acerva.Web.Models.Home;
using Acerva.Web.Models.Referencia;
using FluentNHibernate.Cfg;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Event;

namespace Acerva.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ISessionFactory SessionFactory = ConfiguraSessionFactoryPersistencia();
        public static readonly string PastaLog = HostingEnvironment.MapPath("~/Log");
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Version _versaoSistema = Assembly.GetAssembly(typeof(MvcApplication)).GetName().Version;
        private static readonly string _versaoSistemaFormatada = string.Format("{0}.{1}.{2}", _versaoSistema.Major, _versaoSistema.Minor, _versaoSistema.Build);

        public static Version VersaoSistema
        {
            get { return _versaoSistema; }
        }

        public static string VersaoSistemaFormatada
        {
            get { return _versaoSistemaFormatada; }
        }

        public static ISession CurrentSession
        {
            get { return DependencyResolver.Current.GetService<ISession>(); }
        }

        public static ISession NewSession
        {
            get { return SessionFactory.OpenSession(); }
        }

        private static ISessionFactory ConfiguraSessionFactoryPersistencia()
        {
            var sessionFactory = Fluently.Configure()
                .Mappings(
                    m => m.FluentMappings.AddFromAssemblyOf<RegionalClassMap>()
                )
                .ExposeConfiguration(
                    config =>
                    {
                        config.EventListeners.PostCommitUpdateEventListeners = new IPostUpdateEventListener[] { new StatusEventListener() };
                        config.EventListeners.PostCommitInsertEventListeners = new IPostInsertEventListener[] { new StatusEventListener() };
                    })
                .BuildSessionFactory();

            return sessionFactory;
        }

        protected void Application_Start()
        {
            ConfiguraLog();

            Log.Info("Application Start");

            Log.InfoFormat("Inicializando sistema '{0}'", Sistema.NomeSistema);
            Log.InfoFormat("Versão {0}", VersaoSistemaFormatada);

            // desabilita o header http X-AspNetMvc-Version
            MvcHandler.DisableMvcResponseHeader = true;

            ConfiguraMvc();
            ConfiguraGlobalization();
            ConfiguraAutoMapper();
        }
        
        public MvcApplication()
        {
            BeginRequest += MvcApplication_BeginRequest;
            EndRequest += MvcApplication_EndRequest;
        }

        private void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            var culture = new CultureInfo("pt-br");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // http://weblog.west-wind.com/posts/2009/Apr/29/IIS-7-Error-Pages-taking-over-500-Errors#EnterResponse.TrySkipIisCustomErrors
            var application = (HttpApplication)sender;
            application.Context.Response.TrySkipIisCustomErrors = true;
        }

        private static void MvcApplication_EndRequest(object sender, EventArgs e)
        {
        }

        private static void ConfiguraLog()
        {
            XmlConfigurator.Configure();
            Log.Debug("Log4Net configurado");
        }
        
        private static void ConfiguraMvc()
        {
            Log.Info("Configurando Mvc");

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.DefaultBinder = new ModelBinder();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            Log.Debug("Mvc configurado");
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("admin/{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }


        private static void ConfiguraGlobalization()
        {
            Log.Info("Configurando Globalization");

            DefaultModelBinder.ResourceClassKey = "Messages";
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(CustomRequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(CustomRangeAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthAttribute), typeof(CustomStringLengthAttributeAdapter));

            Log.Debug("Globalization configurado");
        }

        private static void ConfiguraAutoMapper()
        {
            Log.Info("Configurando AutoMapper");

            // https://github.com/AutoMapper/AutoMapper/wiki/Configuration
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<HomeMapperProfile>();
                cfg.AddProfile<CadastroRegionalMapperProfile>();
                cfg.AddProfile<CadastroArtigosMapperProfile>();
                cfg.AddProfile<ReferenciaMapperProfile>();
                cfg.AddProfile<CadastroUsuariosMapperProfile>();
            });

            Log.Debug("AutoMapper configurado");
        }

        protected void Application_End()
        {
            Log.InfoFormat("Finalizando sistema '{0}'", Sistema.NomeSistema);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            Log.Error("Application_Error", exception);
        }

        public static void LimpaCacheDeSegundoNivelDaPersistencia()
        {
            SessionFactory.EvictQueries();
            foreach (var collectionMetadata in SessionFactory.GetAllCollectionMetadata())
                SessionFactory.EvictCollection(collectionMetadata.Key);
            foreach (var classMetadata in SessionFactory.GetAllClassMetadata())
                SessionFactory.EvictEntity(classMetadata.Key);
        }
    }
}
