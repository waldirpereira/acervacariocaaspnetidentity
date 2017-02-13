using Acerva.Infra.Repositorios;
using Acerva.Modelo.Mapeamento;
using FluentNHibernate.Cfg;
using NHibernate;
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

            Bind(typeof(ICadastroHistoricoStatusUsuarios))
                .To(typeof(CadastroHistoricoStatusUsuarios))
                .InRequestScope();

            Bind<ISession>()
                .ToMethod(
                    context =>
                    {
                        var lockObject = new object();

                        lock (lockObject)
                        {
                            return Fluently.Configure()
                                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<HistoricoStatusUsuarioClassMap>())
                                .ExposeConfiguration(config => {})
                                .BuildSessionFactory()
                                .OpenSession();
                        }
                    }
                )
                .WhenInjectedInto<ICadastroHistoricoStatusUsuarios>()
                .InThreadScope();

            Bind(typeof(ICadastroArtigos))
                .To(typeof(CadastroArtigos))
                .InRequestScope();
        }

    }
}