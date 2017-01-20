using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroRegionais : ICadastroRegionais
    {
        private readonly ISession _session;

        public CadastroRegionais(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Regional> BuscaParaListagem()
        {
            return _session.Query<Regional>();
        }

        public void Salva(Regional regional)
        {
            if (regional.Codigo <= 0)
                regional.Codigo = BuscaProximoCodigo();

            _session.Flush();
            _session.SaveOrUpdate(regional);
        }
        public int BuscaProximoCodigo()
        {
            var lista = _session.Query<Regional>();
            if (!lista.Any())
                return 1;

            return lista.Max(e => e.Codigo) + 1;
        }

        public Regional Busca(int codigo)
        {
            return _session.Get<Regional>(codigo);
        }

        public IEnumerable<Regional> BuscaTodos()
        {
            return _session.Query<Regional>();
        }
    }
}