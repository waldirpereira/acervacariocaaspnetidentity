using System.Collections.Generic;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroBeneficios : ICadastroBeneficios
    {
        private readonly ISession _session;

        public CadastroBeneficios(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Beneficio> BuscaParaListagem()
        {
            return _session.Query<Beneficio>();
        }

        public void Salva(Beneficio beneficio)
        {
            _session.Flush();
            _session.SaveOrUpdate(beneficio);
        }
        public Beneficio Busca(int codigo)
        {
            return _session.Get<Beneficio>(codigo);
        }

        public IEnumerable<Beneficio> BuscaTodas()
        {
            return _session.Query<Beneficio>();
        }
    }
}