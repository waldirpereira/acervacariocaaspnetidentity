using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Acerva.Modelo.Mapeamento.Types
{
    public class SimNaoType : CharBooleanType
    {
        protected override sealed string TrueString
        {
            get
            {
                return "S";
            }
        }

        protected override sealed string FalseString
        {
            get
            {
                return "N";
            }
        }

        public override string Name
        {
            get
            {
                return "SimNao";
            }
        }

        public SimNaoType()
            : base(new AnsiStringFixedLengthSqlType(1))
        {
        }
    }
}