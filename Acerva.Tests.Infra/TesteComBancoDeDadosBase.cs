using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using System.Data;

namespace Acerva.Tests.Infra
{
    [TestClass]
    public abstract class TesteComBancoDeDadosBase
    {
        protected static ISession Session;

        protected ISessionFactory SessionFactory
        {
            get { return TestBootstrapper.SessionFactory; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Session = SessionFactory.OpenSession();

            HabilitaFiltros();

            Session.BeginTransaction(IsolationLevel.ReadCommitted);
            ExecutaAntesDeCadaTeste();
        }

        protected virtual void HabilitaFiltros()
        {
        }

        protected virtual void ExecutaAntesDeCadaTeste()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ExecutaDepoisDeCadaTeste();
            Session.Transaction.Rollback();
            Session.Dispose();
        }

        protected virtual void ExecutaDepoisDeCadaTeste()
        {
        }

        protected void LimpaCacheDaSession()
        {
            Session.Flush();
            Session.Clear();
        }
    }
}
