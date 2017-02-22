using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Acerva.Modelo
{
    public class Usuario : IUser, IEquatable<Usuario>, IUsuarioHistoricoStatus
    {
        public Usuario() { }

        public Usuario(string userName) : this()
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
        public virtual string Cpf { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual Uf Uf { get; set; }
        public virtual string Cep { get; set; }
        public virtual string TelefoneFixo { get; set; }
        public virtual string Rg { get; set; }
        public virtual DateTime? DataNascimento { get; set; }
        public virtual DateTime? DataAdmissao { get; set; }
        public virtual string EmailLista { get; set; }
        public virtual Sexo? Sexo { get; set; }
        public virtual string Experiencia { get; set; }
        public virtual string Observacao { get; set; }
        public virtual bool EmailBoasVindasListaEnviado { get; set; } 

        private ICollection<Papel> _papeis = new List<Papel>();
        public virtual ICollection<Papel> Papeis {
            get { return _papeis; }
            set { _papeis = value; } }


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

        public virtual HistoricoStatusUsuario GeraGeraHistoricoStatus()
        {
            var historico = new HistoricoStatusUsuario
            {
                IdUsuarioAlterado = Id,
                DataHora = DateTime.Now,
                StatusNovo = Status,
                NomeUsuarioLogado = string.IsNullOrEmpty(Thread.CurrentPrincipal.Identity.Name)
                    ? "desconhecido"
                    : Thread.CurrentPrincipal.Identity.Name
            };

            return historico;
        }
    }
}
