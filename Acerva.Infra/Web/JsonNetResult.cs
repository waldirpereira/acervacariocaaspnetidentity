using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Acerva.Infra.Web
{
    //http://stackoverflow.com/questions/7109967/using-json-net-as-the-default-json-serializer-in-asp-net-mvc-3-is-it-possible
    public class JsonNetResult : JsonResult
    {
        public const int HttpBadRequest = 400;

        public string Content = "";

        private readonly int? _statusCode;
        public virtual int? StatusCode { get { return _statusCode; } }

        public JsonSerializerSettings Settings { get; private set; }

        public JsonNetResult(object json, ReferenceLoopHandling loopHandling = ReferenceLoopHandling.Error, int? statusCode = null)
        {
            _statusCode = statusCode;
            Data = json;
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = loopHandling,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            Settings.Converters.Add(new JsonEnumComAtributosConverter());
        }

        public JsonNetResult() { }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (_statusCode.HasValue)
                response.StatusCode = _statusCode.Value;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            //If you need special handling, you can call another form of SerializeObject below
            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented, Settings);
            Content = serializedObject;
            response.Write(serializedObject);
        }
    }
}
