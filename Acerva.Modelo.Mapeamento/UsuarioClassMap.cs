using FluentNHibernate.Mapping;
using Acerva.Utils;

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
            Map(u => u.AccessFailedCount, "AccessFailedCount");
            Map(u => u.EmailConfirmed, "EmailConfirmed");
            Map(u => u.LockoutEnabled, "LockoutEnabled");
            Map(u => u.LockoutEndDateUtc, "LockoutEndDateUtc");
            Map(u => u.PhoneNumber, "PhoneNumber");
            Map(u => u.PhoneNumberConfirmed, "PhoneNumberConfirmed");
            Map(u => u.SecurityStamp, "SecurityStamp");
            Map(u => u.TwoFactorEnabled, "TwoFactorEnabled");
            Map(u => u.UserName, "UserName");
            Map(u => u.Email, "Email");
            Map(u => u.Matricula, "matricula");
            Map(u => u.CreationDate, "CreationDate");
            Map(u => u.IndicacaoHash, "codigo_indicacao");
            Map(u => u.Status, "status").CustomType(typeof(EnumComCodigoBdMapper<StatusUsuario>));
            Map(u => u.Cpf, "Cpf");

            References(u => u.Regional, "codigo_regional");
            References(u => u.UsuarioIndicacao, "id_indicacao");

            HasManyToMany(u => u.Papeis)
                .Table("userroles")
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("RoleId");
        }
    }
}