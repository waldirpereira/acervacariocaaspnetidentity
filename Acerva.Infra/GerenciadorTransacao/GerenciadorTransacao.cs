using System;
using NHibernate;
using NHibernate.Transaction;

namespace Acerva.Infra.GerenciadorTransacao
{
    public class GerenciadorTransacao : IGerenciadorTransacao
    {

        class TransacaoComSeucessoSynchronization : ISynchronization
        {
            private readonly Action _action;

            public TransacaoComSeucessoSynchronization(Action action)
            {
                _action = action;
            }

            public void BeforeCompletion()
            {

            }

            public void AfterCompletion(bool success)
            {
                if (success)
                    _action();
            }
        }

        private readonly ISession _session;
        public delegate void FunctionPostCommit();
        public GerenciadorTransacao(ISession session)
        {
            _session = session;
        }

        public void RegistraPostCommit(Action f)
        {
            _session.Transaction.RegisterSynchronization(new TransacaoComSeucessoSynchronization(f));

        }
    }
}
