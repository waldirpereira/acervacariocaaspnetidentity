using System.Collections.Generic;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroVotacoes : ICadastroVotacoes
    {
        private readonly ISession _session;

        public CadastroVotacoes(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Votacao> BuscaParaListagem()
        {
            return _session.Query<Votacao>();
        }

        public void Salva(Votacao votacao)
        {
            _session.Flush();
            _session.SaveOrUpdate(votacao);
        }
        public Votacao Busca(int codigo)
        {
            return _session.Get<Votacao>(codigo);
        }

        public IEnumerable<Votacao> BuscaTodas()
        {
            return _session.Query<Votacao>();
        }
    }
}