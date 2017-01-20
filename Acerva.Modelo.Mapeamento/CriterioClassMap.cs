using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class CriterioClassMap : ClassMap<Criterio>
    {
        public CriterioClassMap()
        {
            Table("criterio");
            Cache.ReadWrite();

            Id(r => r.Codigo, "codigo_criterio").GeneratedBy.Assigned();
            Map(r => r.Ordem, "ordem");
            Map(r => r.Nome, "nome");
            Map(r => r.PontuacaoPadrao, "pontuacao_padrao");
        }
    }
}
