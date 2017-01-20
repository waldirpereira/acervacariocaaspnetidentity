using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Elmah;
using log4net;
using MySql.Data.MySqlClient;

namespace Acerva.Web.AttributeAdapters
{
    [ExcludeFromCodeCoverage]
    public sealed class HandleErrorFazendoLogAttribute : HandleErrorAttribute
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var exception = filterContext.Exception;
            Log.Error(exception.Message, exception);

            RaiseErrorSignal(exception);

            var mensagem = exception.Message;
            if (EhExcecaoMySQL(exception))
            {
                mensagem = "Houve um problema na conexão com a fonte de dados";
            }

            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            filterContext.Result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    message = mensagem
                }
            };
        }

        public static bool EhExcecaoMySQL(Exception exception)
        {
            return exception is MySqlException ||
                (exception is TimeoutException && exception.Source.ToUpperInvariant().Contains("MYSQL"));
        }

        private static void RaiseErrorSignal(Exception e)
        {
            var context = HttpContext.Current;
            ErrorSignal.FromContext(context).Raise(e, context);
        }
    }
}