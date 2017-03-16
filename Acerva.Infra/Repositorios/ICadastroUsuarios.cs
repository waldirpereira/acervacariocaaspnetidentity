using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroUsuarios
    {
        Usuario Busca(string id);
        void SalvaNovo(Usuario usuario);
        Usuario BuscaPeloEmail(string email);
        IEnumerable<Usuario> BuscaTodos();
        IEnumerable<Usuario> BuscaParaListagem(bool incluiCancelados = false);
        IEnumerable<Usuario> BuscaComTermo(string termo);
        bool ExisteComMesmoNome(Usuario usuario);
        void BeginTransaction();
        void Commit();
        void Rollback();
        void Atualiza(Usuario usuario);
        IEnumerable<Usuario> BuscaUsuariosIndicados(string id);
        string PegaProximaMatricula();
        IEnumerable<Papel> BuscaTodosPapeis();
        Usuario BuscaPeloCpf(string cpf);
        bool ExisteComMesmoCpf(Usuario usuario);
        IEnumerable<Uf> BuscaUfs();
        IEnumerable<Usuario> BuscaDelegadosDaRegional(int codigo);
    }
}