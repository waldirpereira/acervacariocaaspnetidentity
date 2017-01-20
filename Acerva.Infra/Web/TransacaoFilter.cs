using System.Web.Mvc;
using NHibernate;
using log4net;

namespace Acerva.Infra.Web
{
    public class TransacaoFilter : IActionFilter
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISession _session;

        public TransacaoFilter(ISession session)
        {
            _session = session;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _session.BeginTransaction();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!_session.Transaction.IsActive)
                return;

            if (filterContext.Exception != null)
            {
                Log.Debug("Executando rollback da transação pois foi lançada uma exceção");
                _session.Transaction.Rollback();
                return;
            }

            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                Log.Debug("Executando rollback da transação pois o ModelState não está válido");
                _session.Transaction.Rollback();
                return;
            }

            var jsonResult = filterContext.Result as JsonNetResult;
            if (jsonResult != null && jsonResult.StatusCode.HasValue && jsonResult.StatusCode.Value >= JsonNetResult.HttpBadRequest)
            {
                Log.Debug("Executando rollback da transação pois o json tem status code = " + jsonResult.StatusCode.Value);
                _session.Transaction.Rollback();
                return;
            }

            Log.Debug("Executando commit da transação");
            _session.Transaction.Commit();

        }
    }
}
