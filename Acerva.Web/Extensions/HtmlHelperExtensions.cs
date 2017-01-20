using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Xml.Linq;
using Acerva.Infra.Web;
using Newtonsoft.Json;

namespace Acerva.Web.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, object routeValues)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues);
            var finalTag = link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(finalTag);
        }

        public static MvcHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, string controller, object routeValues)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            var link = helper.ActionLink("[replaceme]", actionName, controller, routeValues, null);
            var finalTag = link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(finalTag);
        }

        public static MvcHtmlString SpanActionLink(this HtmlHelper helper, string spanStyleClass, string altText, string actionName, string controller, object routeValues)
        {
            var spanBuilder = new TagBuilder("span");
            spanBuilder.AddCssClass(spanStyleClass);
            var link = helper.ActionLink("[replaceme]", actionName, controller, routeValues, new { title = altText });
            var finalTag = link.ToString().Replace("[replaceme]", spanBuilder.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(finalTag);
        }

        public static MvcHtmlString TextForBoolean(this HtmlHelper helper, bool value)
        {
            var text = value ? "Sim" : "Não";
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString TextForBoolean(this HtmlHelper helper, bool? nullableValue)
        {
            return nullableValue.HasValue ? TextForBoolean(helper, nullableValue.Value) : MvcHtmlString.Create("N/D");
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            var tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty, TOpcao>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<TOpcao> listaDeOpcoes, Func<TOpcao, object> funcaoParaPegarValor,
            Func<TOpcao, object> funcaoParaPegarTexto)
        {
            return htmlHelper.DropDownListFor(expression, listaDeOpcoes, funcaoParaPegarValor, funcaoParaPegarTexto, null, false);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty, TOpcao>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<TOpcao> listaDeOpcoes, Func<TOpcao, object> funcaoParaPegarValor, Func<TOpcao, object> funcaoParaPegarTexto, Func<TOpcao, object> funcaoParaOrdenar,
            bool incluirOpcaoSelecione, object htmlAttributes = null)
        {
            var selectList = GeraListaDeSelectListItems(listaDeOpcoes, funcaoParaPegarValor, funcaoParaPegarTexto, funcaoParaOrdenar, incluirOpcaoSelecione);

            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty, TOpcao>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<TOpcao> listaDeOpcoes, Func<TOpcao, object> funcaoParaPegarValor, Func<TOpcao, object> funcaoParaPegarTexto, bool incluirOpcaoSelecione,
            object htmlAttributes = null)
        {
            return htmlHelper.DropDownListFor(expression, listaDeOpcoes, funcaoParaPegarValor, funcaoParaPegarTexto, null, incluirOpcaoSelecione, htmlAttributes);
        }

        public static MvcHtmlString DropDownList<TOpcao>(this HtmlHelper htmlHelper, string name, IEnumerable<TOpcao> listaDeOpcoes,
            Func<TOpcao, object> funcaoParaPegarValor, Func<TOpcao, object> funcaoParaPegarTexto, bool incluirOpcaoSelecione, object htmlAttributes = null)
        {
            var selectList = GeraListaDeSelectListItems(listaDeOpcoes, funcaoParaPegarValor, funcaoParaPegarTexto, null, incluirOpcaoSelecione);

            return htmlHelper.DropDownList(name, selectList, htmlAttributes);
        }

        public static List<SelectListItem> GeraListaDeSelectListItems<TOpcao>(IEnumerable<TOpcao> listaDeOpcoes, Func<TOpcao, object> funcaoParaPegarValor,
            Func<TOpcao, object> funcaoParaPegarTexto, Func<TOpcao, object> funcaoParaOrdenar, bool incluirOpcaoSelecione)
        {
            var selectList = new List<SelectListItem>();

            if (incluirOpcaoSelecione)
                selectList.Add(new SelectListItem { Text = @"Selecione...", Value = "0" });

            if (listaDeOpcoes != null)
            {
                if (funcaoParaOrdenar == null)
                    funcaoParaOrdenar = funcaoParaPegarTexto;

                selectList.AddRange(
                    listaDeOpcoes
                        .OrderBy(funcaoParaOrdenar)
                        .Select(opcao => new SelectListItem
                        {
                            Value = funcaoParaPegarValor.Invoke(opcao).ToString(),
                            Text = funcaoParaPegarTexto.Invoke(opcao).ToString()
                        })
                    );
            }
            return selectList;
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.DropDownListForEnum(expression, null, null);
        }
        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.DropDownListForEnum(expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownListForEnum(expression, null, htmlAttributes);
        }
        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string optionLabel)
        {
            return htmlHelper.DropDownListForEnum(expression, optionLabel, null);
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentNullException("expression");

            var selectedValue = string.Empty;
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata.Model != null)
            {
                selectedValue = metadata.Model.ToString();
            }
            var enumType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

            var listItems = MontaListaItensEnumComNomeExibicao(enumType, selectedValue);

            return htmlHelper.DropDownListFor(expression, listItems, optionLabel, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> MontaListaItensEnumComNomeExibicao(Type enumType, string selectedValue)
        {
            return (from name in Enum.GetNames(enumType)
                    let type = Enum.Parse(enumType, name) as Enum
                    where type != null
                    select new SelectListItem
                    {
                        Text = NomeExibicaoAttribute.GetNome(type) ?? type.ToString(),
                        Value = name,
                        Selected = name == selectedValue
                    }).ToList();
        }

        public static MvcHtmlString CustomDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<CustomSelectItem> selectList, string optionLabel = null, string cssClassSelect = null)
        {
            if (selectList == null)
                return null;

            var selectListAsList = selectList.ToList();

            var dropdown = optionLabel != null
                ? html.DropDownListFor(expression, selectListAsList, optionLabel)
                : html.DropDownListFor(expression, selectListAsList);

            var selectDoc = XDocument.Parse(dropdown.ToString());

            IncludeHtmlAttributesInSelectOptions(selectDoc, selectListAsList, cssClassSelect);

            return MvcHtmlString.Create(selectDoc.ToString());
        }

        public static void IncludeHtmlAttributesInSelectOptions(XDocument selectDoc, IEnumerable<CustomSelectItem> selectList, string cssClassSelect = null)
        {
            if (selectDoc == null)
                return;

            var xSelect = selectDoc.Element("select");
            if (xSelect == null)
                return;

            if (!string.IsNullOrEmpty(cssClassSelect))
            {
                xSelect.SetAttributeValue("class", cssClassSelect);
            }

            var options = (from XElement el in xSelect.Descendants()
                           select el).ToList();

            var listaDeSelectItem = selectList.ToList();
            foreach (var item in options)
            {
                var itemValue = item.Attribute("value");
                if (itemValue == null || string.IsNullOrEmpty(itemValue.Value))
                    continue;
                var customSelectItem = listaDeSelectItem.SingleOrDefault(x => x.Value == itemValue.Value);
                if (customSelectItem == null || customSelectItem.htmlAttributes == null)
                    continue;

                var htmlAttributes = customSelectItem.htmlAttributes;
                foreach (var htmlAttribute in htmlAttributes)
                {
                    item.SetAttributeValue(htmlAttribute.Key, htmlAttribute.Value);
                }
            }

            // rebuild the control, resetting the options with the ones you modified
            if (selectDoc.Root != null)
                selectDoc.Root.ReplaceNodes(options.ToArray());
        }


        public static MvcHtmlString CustomDropDownList(this HtmlHelper html, string nome, IEnumerable<CustomSelectItem> selectList, string optionLabel, string cssClassSelect = null)
        {
            if (selectList == null)
                return null;

            var listaDeSelectItem = selectList.ToList();
            var selectDoc = XDocument.Parse(html.DropDownList(nome, listaDeSelectItem, optionLabel).ToString());

            IncludeHtmlAttributesInSelectOptions(selectDoc, listaDeSelectItem, cssClassSelect);

            return MvcHtmlString.Create(selectDoc.ToString());
        }

        public static MvcHtmlString RadioButtonsForBoolean<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression,
            bool includeNullOption = true)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentNullException("expression");

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(MontaRadioForExpression(html, expression, metadata.PropertyName, true, true));
            stringBuilder.Append(MontaRadioForExpression(html, expression, metadata.PropertyName, false, true));

            if (!metadata.IsNullableValueType || !includeNullOption)
                return MvcHtmlString.Create(stringBuilder.ToString());

            // é necessário verificar se a expressão vai dar null
            // se for o caso, deve-se for o checked do radio, pois o RadioButtonFor não está marcando o radio quando é null
            var func = expression.Compile();
            var mustBeChecked = func(html.ViewData.Model) == null;

            stringBuilder.Append(MontaRadioForExpression(html, expression, metadata.PropertyName, null, true, mustBeChecked));

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static string MontaRadioForExpression<TModel, TProperty>(HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string radioButtonId,
            bool? nullableBoolValue, bool comLabel = false, bool forceChecked = false)
        {
            string label, idSuffix;
            object value;

            if (nullableBoolValue.HasValue)
            {
                value = nullableBoolValue.Value;
                if (nullableBoolValue.Value)
                {
                    label = "Sim";
                    idSuffix = "Sim";
                }
                else
                {
                    label = "Não";
                    idSuffix = "Nao";
                }
            }
            else
            {
                value = string.Empty;
                label = "N/D";
                idSuffix = "ND";
            }

            var radioButtonIdWithLabel = radioButtonId + idSuffix;
            var htmlAttributes = new Dictionary<string, object>();

            htmlAttributes["id"] = radioButtonIdWithLabel;
            if (forceChecked)
                htmlAttributes["checked"] = "checked";

            var radioHtml = html.RadioButtonFor(expression, value, htmlAttributes);
            if (comLabel)
                return radioHtml + string.Format("<label for=\"{0}\">{1}</label>", radioButtonIdWithLabel, label);

            return radioHtml + label;
        }

        public static string MontaRadio<TModel>(HtmlHelper<TModel> html, string name, bool? value, bool isChecked, bool comLabel = false)
        {
            string label, idSuffix;

            if (value.HasValue)
            {
                value = value.Value;
                if (value.Value)
                {
                    label = "Sim";
                    idSuffix = "Sim";
                }
                else
                {
                    label = "Não";
                    idSuffix = "Nao";
                }
            }
            else
            {
                label = "N/D";
                idSuffix = "ND";
            }

            var radioButtonIdWithLabel = name + idSuffix;
            var htmlAttributes = new Dictionary<string, object>();

            htmlAttributes["id"] = radioButtonIdWithLabel;

            var valorRadio = value.HasValue ? (object)value.Value : string.Empty;
            var radioHtml = html.RadioButton(name, valorRadio, isChecked, htmlAttributes);
            if (comLabel)
                return radioHtml + string.Format("<label for=\"{0}\">{1}</label>", radioButtonIdWithLabel, label);

            return radioHtml + label;
        }

        public static MvcHtmlString GeraJsonDeEnums(this HtmlHelper helper, params Type[] types) //where T : struct, IConvertible
        {
            using (var sw = new StringWriter())
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartObject();

                    foreach (var type in types.Where(t => t.IsEnum))
                    {
                        EscreveJsonDeUmValorDeEnum(writer, type);
                    }

                    writer.WritePropertyName("toArrayOfEnums");
                    writer.WriteRawValue("function (e) { var a = []; _.forOwn(e, function (value, key) { a.push(value); }); return a; }");

                    writer.WriteEndObject();

                    return new MvcHtmlString(sw.ToString());
                }
            }
        }

        private static void EscreveJsonDeUmValorDeEnum(JsonWriter writer, Type type)
        {
            var converter = new JsonEnumComAtributosConverter();
            var serializer = JsonSerializer.Create();

            writer.WritePropertyName(PrimeiraMinuscula(type.Name));

            writer.WriteStartObject();

            foreach (var enumValue in Enum.GetValues(type))
            {
                var nome = enumValue.ToString();
                var nomeCamelCase = PrimeiraMinuscula(nome);

                writer.WritePropertyName(nomeCamelCase);
                converter.WriteJson(writer, enumValue, serializer);
            }

            writer.WriteEndObject();
        }

        private static string PrimeiraMinuscula(string nome)
        {
            return nome.Substring(0, 1).ToLower() + nome.Substring(1);
        }

        public static string AbsoluteAction(this UrlHelper url, string action, string controller)
        {
            var requestUrl = url.RequestContext.HttpContext.Request.Url;

            var absoluteAction = string.Format("{0}://{1}{2}",
                                                  requestUrl.Scheme,
                                                  requestUrl.Authority,
                                                  url.Action(action, controller));

            return absoluteAction;
        }
    }

    public class CustomSelectItem : SelectListItem
    {
        // ReSharper disable once InconsistentNaming
        public IDictionary<string, object> htmlAttributes { get; set; }
    }
}
