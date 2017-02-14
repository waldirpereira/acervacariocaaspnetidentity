using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroCategoriasArtigos : ICadastroCategoriasArtigos
    {
        private readonly ISession _session;

        public CadastroCategoriasArtigos(ISession session)
        {
            _session = session;
        }

        public IEnumerable<CategoriaArtigo> BuscaParaListagem()
        {
            return _session.Query<CategoriaArtigo>();
        }

        public void Salva(CategoriaArtigo categoriaArtigo)
        {
            if (categoriaArtigo.Codigo <= 0)
                categoriaArtigo.Codigo = BuscaProximoCodigo();

            _session.Flush();
            _session.SaveOrUpdate(categoriaArtigo);
        }
        public int BuscaProximoCodigo()
        {
            var lista = _session.Query<CategoriaArtigo>();
            if (!lista.Any())
                return 1;

            return lista.Max(e => e.Codigo) + 1;
        }

        public CategoriaArtigo Busca(int codigo)
        {
            return _session.Get<CategoriaArtigo>(codigo);
        }

        public IEnumerable<CategoriaArtigo> BuscaTodos()
        {
            return _session.Query<CategoriaArtigo>();
        }
    }
}