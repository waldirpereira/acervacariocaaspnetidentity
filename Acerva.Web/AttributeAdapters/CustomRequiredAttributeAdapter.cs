using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Acerva.Web.AttributeAdapters
{
    [ExcludeFromCodeCoverage]
    public class CustomRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public CustomRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute) : base(metadata, context, attribute)
        {
            CustomAttributeAdapterUtil.ConfiguraAdapterMessage(attribute, "PropertyValueRequired");
        }
    }
}