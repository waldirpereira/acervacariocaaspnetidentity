using System.Collections;
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
        IEnumerable<Usuario> BuscaParaListagem();
        IEnumerable<Usuario> BuscaComTermo(string termo);
        bool ExisteComMesmoNome(Usuario usuario);
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}