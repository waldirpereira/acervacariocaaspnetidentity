using System.Linq;
using System.Web.Mvc;

namespace Acerva.Infra.Web
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
            System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    value = stringValue.Trim();
                }
                else
                {
                    value = null;
                }
            }

            base.SetProperty(controllerContext, bindingContext,
                                propertyDescriptor, value);
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!EhTipoDefinidoNoAcerva(bindingContext) || bindingContext.ModelMetadata.ContainerType == null)
                return base.BindModel(controllerContext, bindingContext);

            var form = controllerContext.HttpContext.Request.Form;
            var existeValor = form.AllKeys.Any(k => k.Contains(bindingContext.ModelName) &&
                                                    k.Contains(".") &&
                                                    !string.IsNullOrEmpty(form[k]) &&
                                                    !form[k].Equals("0"));

            return !existeValor ? null : base.BindModel(controllerContext, bindingContext);
        }

        private static bool EhTipoDefinidoNoAcerva(ModelBindingContext bindingContext)
        {
            var ns = bindingContext.ModelMetadata.ModelType.Namespace;
            return ns != null && (bindingContext.ModelMetadata.IsComplexType && ns.Contains("Acerva"));
        }
    }
}