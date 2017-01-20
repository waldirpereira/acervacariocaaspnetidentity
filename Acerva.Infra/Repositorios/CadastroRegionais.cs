using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroRegionais : ICadastroRegionais
    {
        private readonly ISession _session;

        public CadastroRegionais(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Regional> BuscaParaListagem()
        {
            return _session.Query<Regional>();
        }

        public void Salva(Regional regional)
        {
            if (regional.Codigo <= 0)
                regional.Codigo = BuscaProximoCodigo();

            _session.Flush();
            _session.SaveOrUpdate(regional);
        }
        public ITransaction BeginTransaction()
        {
            return _session.BeginTransaction();
        }
        public void CommitTransaction(ITransaction transaction)
        {
            transaction.Commit();
        }

        public void SalvaPalpite(Palpite palpite)
        {
            _session.Evict(palpite.Partida);
            _session.Evict(palpite.Criterio);
            _session.Flush();
            _session.SaveOrUpdate(palpite);
        }

        public IEnumerable<Equipe> BuscaEquipesDoRegional(int codigoRegional)
        {
            return _session.Get<Regional>(codigoRegional).Equipes;
        }

        public int BuscaProximoCodigo()
        {
            var lista = _session.Query<Regional>();
            if (!lista.Any())
                return 1;

            return lista.Max(e => e.Codigo) + 1;
        }

        public Regional Busca(int codigo)
        {
            return _session.Get<Regional>(codigo);
        }

        public IEnumerable<Regional> BuscaTodos()
        {
            return _session.Query<Regional>();
        }

        public IEnumerable<Equipe> BuscaEquipes()
        {
            return _session.Query<Equipe>();
        }

        public IEnumerable<Palpite> PegaPalpitesDeUmaPartida(int codigoPartida)
        {
            return _session.Query<Palpite>().Where(p => p.Partida.Codigo == codigoPartida);
        }

        public Regra BuscaRegraDoCriterioParaAcerva(Criterio criterio, Modelo.Acerva acerva)
        {
            var regra = _session.Query<Regra>()
                .FirstOrDefault(r => r.Acerva.Codigo == acerva.Codigo && r.Criterio.Codigo == criterio.Codigo);
            return regra;
        }

        public Rodada BuscaRodada(int codigoRodada)
        {
            return _session.Get<Rodada>(codigoRodada);
        }
    }
}