using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class PapelClassMap : ClassMap<Papel>
    {
        public PapelClassMap()
        {
            Table("roles");
            Cache.ReadWrite();
            
            Id(u => u.Id, "Id").GeneratedBy.Assigned();
            Map(u => u.Name, "Name");
        }
    }
}