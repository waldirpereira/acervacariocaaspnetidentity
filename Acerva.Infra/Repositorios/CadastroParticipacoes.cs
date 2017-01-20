using System;
using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroParticipacoes : ICadastroParticipacoes
    {
        private readonly ISession _session;

        public CadastroParticipacoes(ISession session)
        {
            _session = session;
        }

        public Participacao Busca(int codigoParticipacao)
        {
            return _session.Get<Participacao>(codigoParticipacao);
        }

        public IEnumerable<Partida> BuscaPartidasDoRegional(int codigoRegional)
        {
            return _session.Query<Partida>() .ToList().Where(p => p.Rodada.Regional.Codigo == codigoRegional);
        }

        public void Salva(Participacao participacao)
        {
            participacao.Palpites.ToList().ForEach(p =>
            {
                if (p.PlacarMandante == null && p.PlacarVisitante == null)
                {
                    participacao.Palpites.Remove(p);
                    return;
                }

                if (p.Codigo <= 0)
                    p.Codigo = 0;

                p.DataHoraPalpite = DateTime.Now;
            });

            _session.Flush();
            _session.SaveOrUpdate(participacao);
        }
    }
}