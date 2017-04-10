using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroBeneficios
    {
        IEnumerable<Beneficio> BuscaTodas();
        IEnumerable<Beneficio> BuscaParaListagem();
        void Salva(Beneficio beneficio);
        Beneficio Busca(int codigo);
    }
}