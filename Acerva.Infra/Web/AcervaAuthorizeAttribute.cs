using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Acerva.Infra.Web
{
    public class AcervaAuthorizeAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(Roles))
                throw (new InvalidOperationException("Nenhum papel especificado."));

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.Url == null)
                    throw (new InvalidOperationException("Não há URL na requisição."));
                
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Account",
                    area = ""
                }));
            }

            var roles = Roles.Split(',');
            if (roles.Any(r => filterContext.HttpContext.User.IsInRole(r.Trim())))
                return;

            throw (new UnauthorizedAccessException(string.Format("Você não possui acesso a esta página. Apenas usuários com perfil '{0}' podem visualizar esta página.", Roles)));
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
