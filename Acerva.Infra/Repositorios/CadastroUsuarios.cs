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
        
        public void SalvaNovo(Usuario usuario)
        {
            _session.Save(usuario);
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

        public IEnumerable<Usuario> BuscaComTermo(string termo)
        {
            return _session.Query<Usuario>()
                .Where(p => (p.Name + "|&|" + p.Email).ToLower().Contains(termo.ToLower()))
                .ToList();
        }

        public bool ExisteComMesmoNome(Usuario usuario)
        {
            var nomeUpper = usuario.Name.ToUpperInvariant();
            var temComMesmoNome = BuscaTodos()
                .Any(e => e.Name.ToUpperInvariant() == nomeUpper && e.Id != usuario.Id);

            return temComMesmoNome;
        }

        public void BeginTransaction()
        {
            _session.BeginTransaction();
        }

        public void Commit()
        {
            _session.Transaction.Commit();
        }

        public void Rollback()
        {
            _session.Transaction.Rollback();
        }

        public void Atualiza(Usuario usuario)
        {
            _session.Merge(usuario);
            _session.Flush();
        }

        public IEnumerable<Usuario> BuscaUsuariosIndicados(string id)
        {
            return _session.Query<Usuario>()
                .Where(u => u.UsuarioIndicacao.Id == id);
        }
    }
}