using NHibernate;

namespace Acerva.Infra.Repositorios
{
    public class CadastroPartidas : ICadastroPartidas
    {
        private readonly ISession _session;

        public CadastroPartidas(ISession session)
        {
            _session = session;
        }
    }
}