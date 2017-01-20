using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Acerva.Infra.Web.FlashMessage
{
    public class FlashMessage
    {
        private readonly string _message;
        private readonly string _title;
        private readonly Notification _notification;
        public bool AutoClose { get; set; }
        public int HideTime { get; set; }
        public string ImageUrl { get; set; }

        public FlashMessage(string message, string title, Notification notification, bool autoClose = true) : this(message, title, notification, -1, true) { }

        public FlashMessage(string message, string title, Notification notification, int hideTime, bool autoClose = true)
        {
            _message = message;
            _title = title;
            _notification = notification;
            HideTime = hideTime;
            AutoClose = autoClose;
        }

        public string GenerateCookieParams()
        {
            var parametros = GeraListaDeParametrosPadrao();
            AdicionaParametrosAdicionais(parametros);

            string parametrosSeparados = "({" + string.Join("^", parametros) + "})";

            return parametrosSeparados;
        }

        protected virtual void AdicionaParametrosAdicionais(ICollection<string> parametros)
        {
        }

        private ICollection<string> GeraListaDeParametrosPadrao()
        {
            var list = new List<string>
                       {
                           "'messageTitle': '" + HttpUtility.UrlEncode(_title, Encoding.Default) + "'",
                           "'messageValue': '" + HttpUtility.UrlEncode(_message, Encoding.Default) + "'",
                           "'messageType': '" + _notification.ToString().ToLower() + "'",
                           "'userMustCloseMessage': " + (!AutoClose).ToString().ToLower(),
                           "'hideTime': " + HideTime
                       };

            if (!string.IsNullOrEmpty(ImageUrl))
            {
                list.Add("'imageUrl': '" + ImageUrl + "'");
            }

            return list;
        }
    }
}
