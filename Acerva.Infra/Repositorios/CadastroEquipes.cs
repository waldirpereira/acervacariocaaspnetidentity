using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroEquipes : ICadastroEquipes
    {
        private readonly ISession _session;

        public CadastroEquipes(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Equipe> BuscaParaListagem()
        {
            return _session.Query<Equipe>();
        }

        public void Salva(Equipe equipe)
        {
            _session.SaveOrUpdate(equipe);
        }

        public int BuscaProximoCodigo()
        {
            var lista = _session.Query<Equipe>();
            if (!lista.Any())
                return 1;

            return lista.Max(e => e.Codigo) + 1;
        }

        public Equipe Busca(int codigo)
        {
            return _session.Get<Equipe>(codigo);
        }

        public IEnumerable<Equipe> BuscaTodas()
        {
            return _session.Query<Equipe>();
        }
    }
}