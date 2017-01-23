using System;

namespace Acerva.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class AbstractEnumStringAttribute : Attribute
    {
        protected string Valor { get; private set; }

        protected AbstractEnumStringAttribute(string valor)
        {
            Valor = valor;
        }

        protected static string GetValor<T, TEnumStringAttribute>(T tipo) where TEnumStringAttribute : AbstractEnumStringAttribute
        {
            var field = tipo.GetType().GetField(tipo.ToString());
            if (field == null) return null;

            var enumStringAttributeArray = field.GetCustomAttributes(typeof(TEnumStringAttribute), false) as TEnumStringAttribute[];
            if (enumStringAttributeArray != null && enumStringAttributeArray.Length > 0)
                return enumStringAttributeArray[0].Valor;

            return null;
        }

        protected static T GetEnum<T, TEnumStringAttribute>(string valor) where TEnumStringAttribute : AbstractEnumStringAttribute
        {
            if (valor == null)
                throw new ArgumentNullException("valor");
            foreach (T tipo in Enum.GetValues(typeof(T)))
            {
                string valor1 = AbstractEnumStringAttribute.GetValor<T, TEnumStringAttribute>(tipo);
                if (valor1 != null && valor1.Equals(valor))
                    return tipo;
            }
            throw new ArgumentException(string.Format("Enumerável não encontrado para o tipo '{0}', valor {1}.", (object)typeof(T), (object)valor));
        }
    }
}