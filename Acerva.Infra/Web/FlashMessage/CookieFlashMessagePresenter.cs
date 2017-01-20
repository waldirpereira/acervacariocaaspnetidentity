using System;
using System.Web;

namespace Acerva.Infra.Web.FlashMessage
{
    public static class CookieFlashMessagePresenter
    {
        public static void ShowFlashMessage(FlashMessage flashMessage)
        {
            var context = HttpContext.Current;
            // HttpContext.Current é um método estático e por isso dificulta os testes. Deve ser evitado ao máximo.
            // Entretanto, não estou vendo outra saída aqui.
            if (context == null)
                return;

            var flashCookie = GetCookie(context);
            string cookieValue;

            bool previousMessageExists = flashCookie.Value != null;
            if (previousMessageExists)
            {
                var valueWithoutLastBracket = flashCookie.Value.Substring(0, flashCookie.Value.LastIndexOf("]", StringComparison.Ordinal));
                cookieValue = valueWithoutLastBracket + "," + flashMessage.GenerateCookieParams() + "]";
            }
            else
            {
                cookieValue = "[" + flashMessage.GenerateCookieParams() + "]";
            }

            flashCookie.Value = cookieValue;
            flashCookie.Expires = DateTime.Now.AddDays(1);

            context.Response.Cookies.Add(flashCookie);
        }

        private static HttpCookie GetCookie(HttpContext context)
        {
            var webAddressUtil = new WebAddressUtil(context.Request);

            var cookieName = string.Format("{0}@{1}", "AcervaMessage", webAddressUtil.GetFullWebSiteAddress().ToLower());
            return HttpContext.Current.Response.Cookies[cookieName];
        }
    }
}
