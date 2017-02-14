using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroCategoriasArtigos
    {
        IEnumerable<CategoriaArtigo> BuscaParaListagem();
        void Salva(CategoriaArtigo categoriaArtigo);
        int BuscaProximoCodigo();
        CategoriaArtigo Busca(int codigo);
        IEnumerable<CategoriaArtigo> BuscaTodos();
    }
}