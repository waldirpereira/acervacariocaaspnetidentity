using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Acerva.Infra.Web
{
    // baseado em: https://julianjelfs.wordpress.com/category/mvc/
    public class ValidateAjaxAntiForgeryTokenAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            if (request.HttpMethod != WebRequestMethods.Http.Post)
                return;

            AntiForgery.Validate(CookieValue(request), request.Headers["__RequestVerificationToken"]);
        }

        private static string CookieValue(HttpRequestBase request)
        {
            var cookie = request.Cookies[AntiForgeryConfig.CookieName];
            return cookie != null ? cookie.Value : null;
        }
    }
}
