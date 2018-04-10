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

        public IEnumerable<Usuario> BuscaParaListagem(bool incluiInativos = false)
        {
            return _session.Query<Usuario>()
                .Where(u => (incluiInativos || (!incluiInativos && u.Status != StatusUsuario.Inativo)));
        }

        public IEnumerable<Usuario> BuscaComTermo(string termo)
        {
            return _session.Query<Usuario>()
                .Where(p => (p.Matricula + "|&|" + p.Name + "|&|" + p.Email).ToLower().Contains(termo.ToLower()))
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

        private static int? NullableTryParseInt32(string text)
        {
            int value;
            return int.TryParse(text, out value) ? (int?)value : null;
        }

        public string PegaProximaMatricula()
        {
            var matriculasNumericas = _session.Query<Usuario>()
                .Select(u => u.Matricula)
                .Select(NullableTryParseInt32);

            var proximaMatricula = matriculasNumericas.Max(m => m ?? 0) + 1;

            return proximaMatricula.ToString();
        }

        public IEnumerable<Papel> BuscaTodosPapeis()
        {
            return _session.Query<Papel>();
        }

        public Usuario BuscaPeloCpf(string cpf)
        {
            return _session.Query<Usuario>()
                .FirstOrDefault(u => u.Cpf.Trim() == cpf.Trim());
        }

        public bool ExisteComMesmoCpf(Usuario usuario)
        {
            var temComMesmoCpf = BuscaTodos()
                .Any(e => !string.IsNullOrEmpty(e.Cpf) && !string.IsNullOrEmpty(usuario.Cpf) && e.Id != usuario.Id && e.Cpf.ToUpperInvariant() == usuario.Cpf.ToUpperInvariant());

            return temComMesmoCpf;
        }

        public IEnumerable<Uf> BuscaUfs()
        {
            return _session.Query<Uf>();
        }

        public IEnumerable<Usuario> BuscaDelegadosDaRegional(int codigo)
        {
            return _session.Query<Usuario>()
                .ToList()
                .Where((u => u.EstaAssociado && u.Regional.Codigo == codigo && u.EhDelegado));
        }

        public IEnumerable<HistoricoStatusUsuario> BuscaHistoricoStatus(string id)
        {
            return _session.Query<HistoricoStatusUsuario>()
                .Where(h => h.IdUsuarioAlterado == id);
        }
    }
}