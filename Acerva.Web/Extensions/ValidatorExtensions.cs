using System.Linq;
using Acerva.Infra.Web;
using FluentValidation.Results;

namespace Acerva.Web.Extensions
{
    public static class ValidatorExtensions
    {
        public static string GeraListaHtmlDeValidacoes(this ValidationResult validationResult)
        {
            var mensagemValidacao = string.Format("Existem erros de validação:<ul><li>{0}</li></ul>",
                    validationResult.Errors
                        .Select(e => string.Format(HtmlEncodeFormatProvider.Instance, "{0:unsafe}", e.ErrorMessage))
                        .DefaultIfEmpty()
                        .Aggregate((a, b) => a + "</li><li>" + b));

            return mensagemValidacao;
        }
    }
}