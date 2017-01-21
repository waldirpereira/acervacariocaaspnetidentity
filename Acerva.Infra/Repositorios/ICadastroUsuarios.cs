using System.Collections;
using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroUsuarios
    {
        IdentityUser Busca(string id);
        void Salva(IdentityUser usuario);
        IdentityUser BuscaPeloEmail(string email);
        IEnumerable<IdentityUser> BuscaTodos();
        IEnumerable<IdentityUser> BuscaParaListagem();
    }
}