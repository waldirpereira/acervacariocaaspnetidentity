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
            _session.SaveOrUpdate(artigo);
            _session.Flush();
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

        public void SalvaAnexo(AnexoArtigo anexo)
        {
            _session.SaveOrUpdate(anexo);
            _session.Flush();
        }

        public AnexoArtigo BuscaAnexo(int codigoAnexo)
        {
            return _session.Get<AnexoArtigo>(codigoAnexo);
        }

        public void ExcluiAnexo(AnexoArtigo anexo)
        {
            _session.Delete(anexo);
        }
    }
}