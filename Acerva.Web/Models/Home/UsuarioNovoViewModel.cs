using System.ComponentModel.DataAnnotations;
using System.Web;
using Acerva.Modelo;

namespace Acerva.Web.Models.Home
{
    public class UsuarioNovoViewModel
    {
        public virtual string Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string FotoBase64 { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual UsuarioIndicacaoViewModel UsuarioIndicacao { get; set; }
    }
}