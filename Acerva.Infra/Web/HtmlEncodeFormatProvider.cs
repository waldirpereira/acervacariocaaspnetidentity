using System;
using System.Web;

namespace Acerva.Infra.Web
{
    // https://msdn.microsoft.com/en-us/library/0asazeez(v=vs.90).aspx
    public class HtmlEncodeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private static readonly HtmlEncodeFormatProvider _instance = new HtmlEncodeFormatProvider();
        public static HtmlEncodeFormatProvider Instance { get { return _instance; } }

        private HtmlEncodeFormatProvider() { }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (arg == null)
                return null;

            if (format == null)
                return string.Format("{0}", arg);

            if (format.StartsWith("unsafe"))
                return HttpUtility.HtmlEncode(arg);

            var formattable = arg as IFormattable;
            return formattable != null ? formattable.ToString(format, provider) : arg.ToString();
        }
    }
}

