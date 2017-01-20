using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroParticipacoes
    {
        Participacao Busca(int codigoParticipacao);
        IEnumerable<Partida> BuscaPartidasDoRegional(int codigoRegional);
        void Salva(Participacao participacao);
    }
}