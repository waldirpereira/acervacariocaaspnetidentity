using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroAvioes
    {
        IEnumerable<Aviao> BuscaTodas();
        IEnumerable<Aviao> BuscaParaListagem();
        void Salva(Aviao aviao);
        Aviao Busca(int codigo);
    }
}