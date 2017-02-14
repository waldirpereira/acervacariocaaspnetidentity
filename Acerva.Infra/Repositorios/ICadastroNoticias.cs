using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroNoticias
    {
        IEnumerable<Noticia> BuscaTodas();
        IEnumerable<Noticia> BuscaParaListagem();
        void Salva(Noticia noticia);
        Noticia Busca(int codigo);
        IEnumerable<CategoriaArtigo> BuscaCategorias();
    }
}