using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroAcervas
    {
        IEnumerable<Modelo.Acerva> BuscaParaListagem();
        void Salva(Modelo.Acerva acerva);
        Modelo.Acerva Busca(int codigo);
        IEnumerable<Modelo.Acerva> BuscaTodos();
        IEnumerable<Regional> BuscaRegionais();
        IEnumerable<Modelo.Acerva> BuscaTodosEmQueUsuarioParticipa(string userId);
        IEnumerable<Criterio> BuscaCriterios();
        void BuscaUsuariosJaCadastrados(Modelo.Acerva acerva);
    }
}