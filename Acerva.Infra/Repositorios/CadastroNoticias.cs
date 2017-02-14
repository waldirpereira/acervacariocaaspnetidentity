using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroNoticias : ICadastroNoticias
    {
        private readonly ISession _session;

        public CadastroNoticias(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Noticia> BuscaParaListagem()
        {
            return _session.Query<Noticia>();
        }

        public void Salva(Noticia noticia)
        {
            _session.Flush();
            _session.SaveOrUpdate(noticia);
        }
        public Noticia Busca(int codigo)
        {
            return _session.Get<Noticia>(codigo);
        }

        public IEnumerable<CategoriaArtigo> BuscaCategorias()
        {
            return _session.Query<CategoriaArtigo>();
        }

        public IEnumerable<Noticia> BuscaTodas()
        {
            return _session.Query<Noticia>();
        }
    }
}