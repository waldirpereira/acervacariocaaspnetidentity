using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Acerva.Modelo
{
    public class Usuario : IUser, IEquatable<Usuario>
    {

        /// <summary>
        /// Default constructor 
        /// </summary>
        public Usuario()
        {
            //Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor that takes user name as argument
        /// </summary>
        /// <param name="userName"></param>
        public Usuario(string userName)
            : this()
        {
            UserName = userName;
        }
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual Regional Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual Usuario UsuarioIndicacao { get; set; }
        public virtual string IndicacaoHash { get; set; }
        public virtual string Matricula { get; set; }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Usuario> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity =
                await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual bool Equals(Usuario other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return (GetType() + "|" + Id).GetHashCode();
        }
    }
}
