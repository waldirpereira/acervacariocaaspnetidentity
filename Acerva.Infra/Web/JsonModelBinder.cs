using System;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;

namespace Acerva.Infra.Web
{
    // baseado em 
    // http://www.tomdupont.net/2009/08/mvc-json-model-binder.html
    public class JsonModelBinder : IModelBinder
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly JsonSerializerSettings Settings = CriaJsonSerializerSettings();

        private static JsonSerializerSettings CriaJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            settings.Converters.Add(new JsonEnumComAtributosConverter());

            return settings;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var request = controllerContext.HttpContext.Request;
                var json = request.Params[bindingContext.ModelName];

                if (string.IsNullOrWhiteSpace(json))
                {
                    request.InputStream.Position = 0;
                    using (var inputStream = new StreamReader(request.InputStream))
                    {
                        json = inputStream.ReadToEnd();
                    }

                    if (string.IsNullOrWhiteSpace(json))
                        return null;

                    var jObject = JObject.Parse(json);

                    var conteudo = jObject[bindingContext.ModelName];

                    json = conteudo.ToString();
                }

                // Swap this out with whichever Json deserializer you prefer.
                return JsonConvert.DeserializeObject(json, bindingContext.ModelType, Settings);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
