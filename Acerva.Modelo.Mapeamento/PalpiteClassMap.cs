using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class PalpiteClassMap : ClassMap<Palpite>
    {
        public PalpiteClassMap()
        {
            Table("palpite");
            Cache.ReadWrite();

            Id(p => p.Codigo, "codigo_palpite").GeneratedBy.Increment();

            References(p => p.Participacao, "codigo_participacao");
            References(p => p.Partida, "codigo_partida").Cascade.None();
            References(p => p.Criterio, "codigo_criterio").Cascade.None();

            Map(p => p.DataHoraPalpite, "data_hora_palpite");
            Map(p => p.PlacarMandante, "placar_mandante");
            Map(p => p.PlacarVisitante, "placar_visitante");
            Map(p => p.Pontuacao, "pontuacao");
            Map(p => p.DataHoraPontuacao, "data_hora_pontuacao");
        }
    }
}
