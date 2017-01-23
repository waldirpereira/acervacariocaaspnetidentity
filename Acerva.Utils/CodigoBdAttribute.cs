using System;

namespace Acerva.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodigoBdAttribute : AbstractEnumStringAttribute
    {
        public string Codigo
        {
            get
            {
                return this.Valor;
            }
        }

        public CodigoBdAttribute(string codigo)
          : base(codigo)
        {
        }

        public static string GetCodigo<T>(T tipo)
        {
            return GetValor<T, CodigoBdAttribute>(tipo);
        }

        public static T GetEnumPeloCodigo<T>(string codigo)
        {
            return GetEnum<T, CodigoBdAttribute>(codigo);
        }
    }
}