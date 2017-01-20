using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroRegionais
    {
        IEnumerable<Regional> BuscaParaListagem();
        void Salva(Regional regional);
        int BuscaProximoCodigo();
        Regional Busca(int codigo);
        IEnumerable<Regional> BuscaTodos();
    }
}