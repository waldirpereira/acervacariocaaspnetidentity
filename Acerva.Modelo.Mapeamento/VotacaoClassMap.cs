using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class VotacaoClassMap : ClassMap<Votacao>
    {
        public VotacaoClassMap()
        {
            Table("votacao");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_votacao").GeneratedBy.Increment();
            Map(c => c.Nome, "nome");
            Map(c => c.DataHoraInicio, "data_hora_inicio");
            Map(c => c.DataHoraFim, "data_hora_fim");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));

            HasMany(s => s.Perguntas)
                .KeyColumn("codigo_votacao")
                .Cascade.AllDeleteOrphan();
        }
    }
}
