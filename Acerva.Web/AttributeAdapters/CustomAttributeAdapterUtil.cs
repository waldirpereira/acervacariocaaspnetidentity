using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Resources;

namespace Acerva.Web.AttributeAdapters
{
    [ExcludeFromCodeCoverage]
    public class CustomAttributeAdapterUtil
    {
        public static void ConfiguraAdapterMessage(ValidationAttribute attribute, string propertyKey)
        {
            if (attribute.ErrorMessage == null)
            {
                attribute.ErrorMessageResourceType = typeof(Messages);
                attribute.ErrorMessageResourceName = propertyKey;
            }
        }
    }
}