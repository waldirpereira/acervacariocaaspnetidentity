using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class ParticipacaoClassMap : ClassMap<Participacao>
    {
        public ParticipacaoClassMap()
        {
            Table("participacao");
            Cache.ReadWrite();

            Id(e => e.Codigo, "codigo_participacao").GeneratedBy.Increment();
            Map(p => p.PontuacaoInicial, "pontuacao_inicial");
            Map(p => p.DataHoraInclusao, "data_hora_inclusao");
            References(p => p.Usuario, "codigo_usuario").Cascade.SaveUpdate();
            References(p => p.Acerva, "codigo_acerva");

            HasMany(p => p.Palpites)
                .KeyColumn("codigo_participacao")
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
