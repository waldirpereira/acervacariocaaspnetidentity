using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroArtigos : ICadastroArtigos
    {
        private readonly ISession _session;

        public CadastroArtigos(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Artigo> BuscaParaListagem()
        {
            return _session.Query<Artigo>();
        }

        public void Salva(Artigo artigo)
        {
            _session.Flush();
            _session.SaveOrUpdate(artigo);
        }

        public Artigo Busca(int codigo)
        {
            return _session.Get<Artigo>(codigo);
        }

        public IEnumerable<Artigo> BuscaTodos()
        {
            return _session.Query<Artigo>();
        }

        public IEnumerable<CategoriaArtigo> BuscaCategorias()
        {
            return _session.Query<CategoriaArtigo>();
        }
    }
}