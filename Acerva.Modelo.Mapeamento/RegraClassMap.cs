using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class RegraClassMap : ClassMap<Regra>
    {
        public RegraClassMap()
        {
            Table("regra");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_regra").GeneratedBy.Increment();
            
            Map(c => c.Pontuacao, "pontuacao");
            Map(c => c.DataHoraAlteracao, "data_hora_alteracao");

            References(c => c.Criterio, "codigo_criterio");
            References(c => c.Acerva, "codigo_acerva");
        }
    }
}
