using System;
using System.Linq;
using Acerva.Modelo;
using NHibernate.Linq;
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
                    var ultimoHistorico = session.Query<HistoricoStatusUsuario>()
                        .Where(h => h.IdUsuarioAlterado == historico.IdUsuarioAlterado)
                        .OrderByDescending(h => h.DataHora)
                        .FirstOrDefault();

                    if (ultimoHistorico != null && historico.StatusNovo == ultimoHistorico.StatusNovo)
                        return;

                    if (!string.IsNullOrEmpty(historico.EmailUsuarioLogado))
                        historico.UsuarioLogado = session.Query<Usuario>()
                            .FirstOrDefault(u => u.Email == historico.EmailUsuarioLogado &&
                                        (u.Status == StatusUsuario.Ativo ||
                                         u.Status == StatusUsuario.AguardandoRenovacao));

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