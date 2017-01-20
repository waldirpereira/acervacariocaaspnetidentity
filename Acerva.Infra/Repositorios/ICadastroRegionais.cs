using System.Collections.Generic;
using Acerva.Modelo;
using NHibernate;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroRegionais
    {
        IEnumerable<Regional> BuscaParaListagem();
        void Salva(Regional regional);
        int BuscaProximoCodigo();
        Regional Busca(int codigo);
        IEnumerable<Regional> BuscaTodos();
        IEnumerable<Equipe> BuscaEquipes();
        IEnumerable<Palpite> PegaPalpitesDeUmaPartida(int codigoPartida);
        Regra BuscaRegraDoCriterioParaAcerva(Criterio criterio, Modelo.Acerva acerva);
        Rodada BuscaRodada(int codigoRodada);
        ITransaction BeginTransaction();
        void CommitTransaction(ITransaction transaction);
        void SalvaPalpite(Palpite palpite);
        IEnumerable<Equipe> BuscaEquipesDoRegional(int codigoRegional);
    }
}