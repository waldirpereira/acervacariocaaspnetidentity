using System.Web.Mvc;

namespace Acerva.Infra.Web.FlashMessage
{
    public static class FlashMessageExtensions
    {
        public static ActionResult ErrorMessage(this ActionResult result, string message, string title = "Erro",
            bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Error, message, title, autoClose);
            flash.AutoClose = false;
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        public static ActionResult DefaultMessage(this ActionResult result, string message, string title = "Mensagem",
            bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Default, message, title, autoClose);
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        public static ActionResult SuccessMessage(this ActionResult result, string message, string title = "Sucesso",
            bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Success, message, title, autoClose);
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        public static ActionResult InformationMessage(this ActionResult result, string message,
            string title = "Informação", bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Info, message, title, autoClose);
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        public static ActionResult WarningMessage(this ActionResult result, string message, string title = "Aviso",
            bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Warning, message, title, autoClose);
            flash.AutoClose = false;
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        public static ActionResult ForbiddenMessage(this ActionResult result, string message,
            string title = "Acesso negado", bool autoClose = true)
        {
            var flash = CreateFlashMessage(Notification.Forbidden, message, title, autoClose);
            CookieFlashMessagePresenter.ShowFlashMessage(flash);
            return result;
        }

        private static FlashMessage CreateFlashMessage(Notification notification, string message, string title,
            bool autoClose)
        {
            return new FlashMessage(message, title, notification, autoClose);
        }

        public static ActionResult ShowFlashMessage(this ActionResult result, FlashMessage flashMessage)
        {
            CookieFlashMessagePresenter.ShowFlashMessage(flashMessage);
            return result;
        }
    }
}