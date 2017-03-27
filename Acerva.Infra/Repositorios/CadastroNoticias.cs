using System.Collections.Generic;
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

        public void SalvaAnexo(AnexoNoticia anexo)
        {
            _session.SaveOrUpdate(anexo);
            _session.Flush();
        }

        public AnexoNoticia BuscaAnexo(int codigoAnexo)
        {
            return _session.Get<AnexoNoticia>(codigoAnexo);
        }

        public void ExcluiAnexo(AnexoNoticia anexo)
        {
            _session.Delete(anexo);
        }
    }
}