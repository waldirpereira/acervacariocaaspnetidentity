using NHibernate;

namespace Acerva.Infra.Repositorios
{
    public class CadastroRodadas : ICadastroRodadas
    {
        private readonly ISession _session;

        public CadastroRodadas(ISession session)
        {
            _session = session;
        }
    }
}