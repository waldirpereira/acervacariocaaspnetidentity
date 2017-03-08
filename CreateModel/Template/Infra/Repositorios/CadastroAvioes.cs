using System.Collections.Generic;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroAvioes : ICadastroAvioes
    {
        private readonly ISession _session;

        public CadastroAvioes(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Aviao> BuscaParaListagem()
        {
            return _session.Query<Aviao>();
        }

        public void Salva(Aviao aviao)
        {
            _session.Flush();
            _session.SaveOrUpdate(aviao);
        }
        public Aviao Busca(int codigo)
        {
            return _session.Get<Aviao>(codigo);
        }

        public IEnumerable<Aviao> BuscaTodas()
        {
            return _session.Query<Aviao>();
        }
    }
}