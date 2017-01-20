using Acerva.Modelo.Mapeamento.Types;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class PartidaClassMap : ClassMap<Partida>
    {
        public PartidaClassMap()
        {
            Table("partida");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_partida").GeneratedBy.Increment();

            Map(p => p.PlacarMandante, "placar_mandante");
            Map(p => p.PlacarVisitante, "placar_visitante");
            Map(p => p.DataHora, "data_hora");
            Map(p => p.Terminada, "terminada").CustomType(typeof(SimNaoType));
            
            References(r => r.Rodada, "codigo_rodada");
            References(p => p.EquipeMandante, "codigo_equipe_mandante");
            References(p => p.EquipeVisitante, "codigo_equipe_visitante");
        }
    }
}
