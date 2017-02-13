using System;
using Acerva.Modelo;
using NHibernate;

namespace Acerva.Infra.Repositorios
{
    public class CadastroHistoricoStatusUsuarios : ICadastroHistoricoStatusUsuarios
    {
        private readonly ISession _sessaoEspecifica;

        public CadastroHistoricoStatusUsuarios(ISession sessaoEspecifica)
        {
            _sessaoEspecifica = sessaoEspecifica;
        }

        public void Salva(HistoricoStatusUsuario historico)
        {
            using (var transacao = _sessaoEspecifica.BeginTransaction())
            {
                try
                {
                    _sessaoEspecifica.Save(historico);
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
                }
            }
        }
    }
}