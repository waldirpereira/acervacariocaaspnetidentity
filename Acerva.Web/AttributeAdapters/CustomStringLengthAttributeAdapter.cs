using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Acerva.Web.AttributeAdapters
{
    [ExcludeFromCodeCoverage]
    public class CustomStringLengthAttributeAdapter : StringLengthAttributeAdapter
    {
        public CustomStringLengthAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute) : base(metadata, context, attribute)
        {
            CustomAttributeAdapterUtil.ConfiguraAdapterMessage(attribute, "PropertyStringLengthValue");
        }
    }
}