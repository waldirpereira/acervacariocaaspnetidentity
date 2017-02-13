using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using NHibernate.Event;

namespace Acerva.Infra.Eventos
{
    public class StatusEventListener : IPostUpdateEventListener, IPostInsertEventListener
    {
        private ICadastroHistoricoStatusUsuarios _cadastroHistoricoStatusUsuarios;

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var usuario = @event.Entity as IUsuarioHistoricoStatus;
            if (usuario == null)
                return;

            var historico = usuario.GeraGeraHistoricoStatus();
            GravaRegistroAlteracaoStatus(historico);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var usuario = @event.Entity as IUsuarioHistoricoStatus;
            if (usuario == null)
                return;

            var historico = usuario.GeraGeraHistoricoStatus();
            GravaRegistroAlteracaoStatus(historico);
        }

        private void GravaRegistroAlteracaoStatus(HistoricoStatusUsuario historico)
        {
            if (historico == null)
                return;

            if (_cadastroHistoricoStatusUsuarios == null)
                _cadastroHistoricoStatusUsuarios =
                    System.Web.Mvc.DependencyResolver.Current.GetService(typeof(ICadastroHistoricoStatusUsuarios)) as ICadastroHistoricoStatusUsuarios;

            if (_cadastroHistoricoStatusUsuarios != null)
                _cadastroHistoricoStatusUsuarios.Salva(historico);
        }
    }
}
