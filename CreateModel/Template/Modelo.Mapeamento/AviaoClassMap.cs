using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class AviaoClassMap : ClassMap<Aviao>
    {
        public AviaoClassMap()
        {
            Table("aviao");
            Cache.ReadWrite();

            Id(m => m.Codigo, "codigo_aviao").GeneratedBy.Increment();
            Map(m => m.Nome, "nome");
            Map(m => m.Ativo, "ativo").CustomType(typeof(SimNaoType));
        }
    }
}
