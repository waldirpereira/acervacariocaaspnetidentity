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
        
        public IEnumerable<Noticia> BuscaTodas()
        {
            return _session.Query<Noticia>();
        }
    }
}