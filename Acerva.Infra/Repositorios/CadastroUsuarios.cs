using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroUsuarios : ICadastroUsuarios
    {
        private readonly ISession _session;
        
        public CadastroUsuarios(ISession session)
        {
            _session = session;
        }

        public Usuario Busca(string id)
        {
            return _session.Get<Usuario>(id);
        }

        public void Salva(Usuario usuario)
        {
            _session.Merge(usuario);
            _session.Flush();
        }

        public Usuario BuscaPeloEmail(string email)
        {
            var usuariosComMesmoEmail = _session.Query<Usuario>().Where(u => u.Email == email).ToList();
            return usuariosComMesmoEmail.OrderBy(u => u.CreationDate).LastOrDefault(); //novos usuarios estarão com data 0000-00-00 00:00
        }

        public IEnumerable<Usuario> BuscaTodos()
        {
            return _session.Query<Usuario>();
        }

        public IEnumerable<Usuario> BuscaParaListagem()
        {
            return _session.Query<Usuario>();
        }
    }
}