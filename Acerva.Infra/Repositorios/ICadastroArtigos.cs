using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroArtigos
    {
        IEnumerable<Artigo> BuscaParaListagem();
        void Salva(Artigo artigo);
        Artigo Busca(int codigo);
        IEnumerable<Artigo> BuscaTodos();
        IEnumerable<CategoriaArtigo> BuscaCategorias();
        void SalvaAnexo(AnexoArtigo anexo);
        AnexoArtigo BuscaAnexo(int codigoAnexo);
        void ExcluiAnexo(AnexoArtigo anexo);
    }
}