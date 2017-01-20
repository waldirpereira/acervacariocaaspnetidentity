using System;

namespace Acerva.Infra.Web
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NomeExibicaoAttribute : AbstractEnumStringAttribute
    {
        public string Nome
        {
            get
            {
                return Valor;
            }
        }

        public NomeExibicaoAttribute(string nome)
          : base(nome)
        {
        }

        public static string GetNome<T>(T tipo)
        {
            return AbstractEnumStringAttribute.GetValor<T, NomeExibicaoAttribute>(tipo);
        }

        public static T GetEnumPeloNome<T>(string nome)
        {
            return AbstractEnumStringAttribute.GetEnum<T, NomeExibicaoAttribute>(nome);
        }
    }
}