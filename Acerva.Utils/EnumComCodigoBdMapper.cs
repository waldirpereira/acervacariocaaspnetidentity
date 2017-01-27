using System;
using NHibernate.Type;

namespace Acerva.Utils
{
    public class EnumComCodigoBdMapper<T> : EnumStringType<T>
    {
        public override object GetValue(object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException("enumValue");

            T enumComCodigoASerRecuperado = (T) enumValue;

            return CodigoBdAttribute.GetCodigo(enumComCodigoASerRecuperado);
        }

        public override object GetInstance(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            T enumEncontrado = CodigoBdAttribute.GetEnumPeloCodigo<T>(value.ToString());
            return enumEncontrado;
        }
    }
}
