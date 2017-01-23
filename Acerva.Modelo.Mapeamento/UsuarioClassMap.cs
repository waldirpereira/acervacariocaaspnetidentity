using FluentNHibernate.Mapping;
using SiplexCommon.NHibernate;

namespace Acerva.Modelo.Mapeamento
{
    public class UsuarioClassMap : ClassMap<Usuario>
    {
        public UsuarioClassMap()
        {
            Table("users");
            Cache.ReadWrite();
            
            Id(u => u.Id, "Id").GeneratedBy.Assigned();
            Map(u => u.Name, "Name");
            Map(u => u.PasswordHash, "PasswordHash");
            Map(u => u.UserName, "UserName");
            Map(u => u.Email, "Email");
            Map(u => u.CreationDate, "CreationDate");
            Map(u => u.Status, "status").CustomType(typeof(EnumComCodigoBdMapper<StatusUsuario>));

            References(u => u.Regional, "codigo_regional");
        }
    }
}