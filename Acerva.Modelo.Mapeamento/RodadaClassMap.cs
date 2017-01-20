using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class RodadaClassMap : ClassMap<Rodada>
    {
        public RodadaClassMap()
        {
            Table("rodada");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_rodada").GeneratedBy.Increment();
            Map(c => c.Nome, "nome");
            Map(c => c.Ordem, "ordem");

            References(r => r.Regional, "codigo_regional");

            HasMany(r => r.Partidas)
                .KeyColumn("codigo_rodada")
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
