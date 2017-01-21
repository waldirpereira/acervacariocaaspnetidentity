using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class UsuarioClassMap : ClassMap<Usuario>
    {
        public UsuarioClassMap()
        {
            Table("users");
            Cache.ReadWrite();
            //ReadOnly();

            Id(u => u.Id, "Id").GeneratedBy.Assigned();
            Map(u => u.Name, "Name");
            Map(u => u.PasswordHash, "PasswordHash");
            Map(u => u.UserName, "UserName");
            Map(u => u.Email, "Email");
            Map(u => u.CreationDate, "CreationDate");

            References(u => u.Regional, "codigo_regional");
        }
    }
}