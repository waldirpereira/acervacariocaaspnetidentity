using System;
using Acerva.Modelo;
using NHibernate;

namespace Acerva.Infra.Repositorios
{
    public class CadastroHistoricoStatusUsuarios : ICadastroHistoricoStatusUsuarios
    {
        private readonly ISessionFactory _sessionFactory;

        public CadastroHistoricoStatusUsuarios(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Salva(HistoricoStatusUsuario historico)
        {
            var session = _sessionFactory.OpenSession();
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    session.Save(historico);
                    session.Flush();
                    transacao.Commit();
                }
                catch (Exception)
                {
                    transacao.Rollback();
                    throw;
                }
                finally
                {
                    transacao.Dispose();
                    session.Dispose();
                }
            }
        }
    }
}