using System;
using System.Web;

namespace Acerva.Infra.Web.FlashMessage
{
    public class WebAddressUtil
    {
        private readonly HttpRequest _request;

        public WebAddressUtil(HttpRequest request)
        {
            this._request = request;
        }

        public string GetFullWebSiteAddress()
        {
            string siteUrl = _request.Url.GetLeftPart(UriPartial.Authority) + _request.ApplicationPath;
            //siteUrl = siteUrl.EndsWith("/") ? siteUrl : siteUrl + "/"; //add final slash
            siteUrl = siteUrl.EndsWith("/") ? siteUrl.Remove(siteUrl.Length - 1, 1) : siteUrl; //remove final slash
            return siteUrl;
        }

        public string GetWebDirectoryName()
        {
            string folderUrl = _request.ApplicationPath;

            //folderUrl = folderUrl.EndsWith("/") ? folderUrl : folderUrl + "/"; //add final slash
            folderUrl = folderUrl != null && folderUrl.EndsWith("/") ? folderUrl.Remove(folderUrl.Length - 1, 1) : folderUrl; //remove final slash
            return folderUrl;
        }

        public string GetRequestFolderName()
        {
            string folderUrl = VirtualPathUtility.GetDirectory(_request.Path);
            //folderUrl = folderUrl.EndsWith("/") ? folderUrl : folderUrl + "/"; //add final slash
            folderUrl = folderUrl.EndsWith("/") ? folderUrl.Remove(folderUrl.Length - 1, 1) : folderUrl; //remove final slash
            return folderUrl;
        }

        public string GetRequestPageName()
        {
            return _request.AppRelativeCurrentExecutionFilePath.Replace("~/", "");
        }
    }
}
