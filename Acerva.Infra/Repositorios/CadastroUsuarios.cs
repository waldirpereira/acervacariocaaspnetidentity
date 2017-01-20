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

        public IdentityUser Busca(string id)
        {
            return _session.Get<IdentityUser>(id);
        }

        public void Salva(IdentityUser usuario)
        {
            _session.Merge(usuario);
            _session.Flush();
        }

        public IdentityUser BuscaPeloEmail(string email)
        {
            var usuariosComMesmoEmail = _session.Query<IdentityUser>().Where(u => u.Email == email).ToList();
            return usuariosComMesmoEmail.OrderBy(u => u.CreationDate).LastOrDefault(); //novos usuarios estarão com data 0000-00-00 00:00
        }
    }
}