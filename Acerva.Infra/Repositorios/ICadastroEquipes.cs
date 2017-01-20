using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroEquipes
    {
        IEnumerable<Equipe> BuscaParaListagem();
        void Salva(Equipe equipe);
        int BuscaProximoCodigo();
        Equipe Busca(int codigo);
        IEnumerable<Equipe> BuscaTodas();
    }
}