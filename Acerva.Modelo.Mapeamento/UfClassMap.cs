using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class UfClassMap : ClassMap<Uf>
    {
        public UfClassMap()
        {
            Table("uf");
            Cache.ReadOnly();

            Id(uf => uf.Codigo, "codigo_uf").GeneratedBy.Increment();
            Map(uf => uf.CodigoIbge, "codigo_ibge");
            Map(uf => uf.Sigla, "sigla");
            Map(uf => uf.Nome, "nome");
        }
    }
}
