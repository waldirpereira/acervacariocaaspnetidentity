using System;
using System.Linq;
using Acerva.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Acerva.Infra.Web
{
    public class JsonEnumComAtributosConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var nome = PrimeiraMinuscula(value.ToString());

            writer.WriteStartObject();

            writer.WritePropertyName("codigo");
            writer.WriteValue((int)value);

            writer.WritePropertyName("nome");
            writer.WriteValue(nome);

            var nomeExibicao = NomeExibicaoAttribute.GetNome(value);
            if (!string.IsNullOrEmpty(nomeExibicao))
            {
                writer.WritePropertyName("nomeExibicao");
                writer.WriteValue(nomeExibicao);
            }

            var codigoBd = CodigoBdAttribute.GetCodigo(value);
            if (!string.IsNullOrEmpty(codigoBd))
            {
                writer.WritePropertyName("codigoBd");
                writer.WriteValue(codigoBd);
            }

            writer.WriteEndObject();
        }

        private static string PrimeiraMinuscula(string nome)
        {
            return nome.Substring(0, 1).ToLower() + nome.Substring(1);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = (Nullable.GetUnderlyingType(objectType) != null);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!isNullable)
                    throw new JsonSerializationException();
                return null;
            }

            if (isNullable)
                objectType = Nullable.GetUnderlyingType(objectType);

            var jObject = JObject.Load(reader);
            var codigo = Convert.ToInt32(jObject.SelectToken("codigo"));
            return Enum.GetValues(objectType).Cast<Enum>().FirstOrDefault(e => Convert.ToInt32(e) == codigo);
        }

        public override bool CanConvert(Type objectType)
        {
            // http://stackoverflow.com/questions/32046891/json-net-custom-enum-converter
            var enumType = (Nullable.GetUnderlyingType(objectType) ?? objectType);
            return enumType.IsEnum;
        }
    }
}