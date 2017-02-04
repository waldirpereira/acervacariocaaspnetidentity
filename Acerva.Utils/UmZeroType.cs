using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Acerva.Utils
{
    public class UmZeroType : CharBooleanType
    {
        protected override sealed string TrueString
        {
            get
            {
                return "1";
            }
        }

        protected override sealed string FalseString
        {
            get
            {
                return "0";
            }
        }

        public override string Name
        {
            get
            {
                return "UmZero";
            }
        }

        public UmZeroType()
            : base(new AnsiStringFixedLengthSqlType(1))
        {
        }
    }
}