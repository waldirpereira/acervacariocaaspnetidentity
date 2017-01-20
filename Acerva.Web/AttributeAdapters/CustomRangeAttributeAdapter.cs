using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Acerva.Web.AttributeAdapters
{
    [ExcludeFromCodeCoverage]
    public class CustomRangeAttributeAdapter : RangeAttributeAdapter
    {
        public CustomRangeAttributeAdapter(ModelMetadata metadata, ControllerContext context, RangeAttribute attribute) : base(metadata, context, attribute)
        {
            CustomAttributeAdapterUtil.ConfiguraAdapterMessage(attribute, "PropertyRangeValue");
        }
    }
}