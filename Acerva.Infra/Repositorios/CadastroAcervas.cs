using System;
using System.Collections.Generic;
using System.Linq;
using Acerva.Modelo;
using NHibernate;
using NHibernate.Linq;

namespace Acerva.Infra.Repositorios
{
    public class CadastroAcervas : ICadastroAcervas
    {
        private readonly ISession _session;
        private readonly ICadastroUsuarios _cadastroUsuarios;

        public CadastroAcervas(ISession session, ICadastroUsuarios cadastroUsuarios)
        {
            _session = session;
            _cadastroUsuarios = cadastroUsuarios;
        }

        public IEnumerable<Modelo.Acerva> BuscaParaListagem()
        {
            return _session.Query<Modelo.Acerva>();
        }

        public void Salva(Modelo.Acerva acerva)
        {
            acerva.Regras.ToList().ForEach(r => { r.DataHoraAlteracao = DateTime.Now; });
            acerva.Participacoes.ToList().ForEach(p =>
            {
                if (string.IsNullOrEmpty(p.Usuario.Id)) p.Usuario.Id = Guid.NewGuid().ToString();
            });
            _session.SaveOrUpdate(acerva);
        }

        public Modelo.Acerva Busca(int codigo)
        {
            return _session.Get<Modelo.Acerva>(codigo);
        }

        public IEnumerable<Modelo.Acerva> BuscaTodos()
        {
            return _session.Query<Modelo.Acerva>();
        }

        public IEnumerable<Regional> BuscaRegionais()
        {
            return _session.Query<Regional>();
        }

        public IEnumerable<Modelo.Acerva> BuscaTodosEmQueUsuarioParticipa(string userId)
        {
            var todosAcervas = _session.Query<Modelo.Acerva>();
            var acervasDoUsuario = from b in todosAcervas
                                  where b.Participacoes.Any(acerva => acerva.Usuario != null && acerva.Usuario.Id == userId)
                                  select b;

            return acervasDoUsuario;
        }

        public IEnumerable<Criterio> BuscaCriterios()
        {
            return _session.Query<Criterio>();
        }

        public void BuscaUsuariosJaCadastrados(Modelo.Acerva acerva)
        {
            acerva.Participacoes.ToList().ForEach(participacao =>
            {
                var usuarioAtual = participacao.Usuario;
                var usuarioNoBd = _cadastroUsuarios.BuscaPeloEmail(participacao.Usuario.Email);
                if (usuarioNoBd == null)
                    return;

                participacao.Usuario = usuarioNoBd;
                if (usuarioNoBd.Id != usuarioAtual.Id)
                    _session.Delete(usuarioAtual);
            });
        }
    }
}