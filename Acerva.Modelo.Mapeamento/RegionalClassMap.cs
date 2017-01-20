using Acerva.Modelo.Mapeamento.Types;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class RegionalClassMap : ClassMap<Regional>
    {
        public RegionalClassMap()
        {
            Table("regional");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_regional").GeneratedBy.Assigned();
            Map(c => c.Nome, "nome");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));
        }
    }
}
