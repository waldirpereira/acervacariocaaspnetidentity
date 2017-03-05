using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroVotacoes
    {
        IEnumerable<Votacao> BuscaTodas();
        IEnumerable<Votacao> BuscaParaListagem();
        void Salva(Votacao votacao);
        Votacao Busca(int codigo);
    }
}