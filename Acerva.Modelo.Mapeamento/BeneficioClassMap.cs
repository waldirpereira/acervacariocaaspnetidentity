using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class BeneficioClassMap : ClassMap<Beneficio>
    {
        public BeneficioClassMap()
        {
            Table("beneficio");
            Cache.ReadWrite();

            Id(m => m.Codigo, "codigo_beneficio").GeneratedBy.Increment();
            Map(m => m.Nome, "nome");
            Map(m => m.TextoHtml, "texto_html");
            Map(m => m.Ativo, "ativo").CustomType(typeof(SimNaoType));
        }
    }
}
