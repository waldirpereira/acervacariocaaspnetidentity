using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroNoticias
    {
        IEnumerable<Noticia> BuscaTodas();
    }
}