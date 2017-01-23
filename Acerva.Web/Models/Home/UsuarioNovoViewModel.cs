using System.ComponentModel.DataAnnotations;
using Acerva.Modelo;

namespace Acerva.Web.Models.Home
{
    public class UsuarioNovoViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = @"Nome")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = @"E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = @"A {0} precisa ter pelo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = @"Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirmação da senha")]
        [Compare("Password", ErrorMessage = @"A senha e a confirmação precisam ser iguais.")]
        public string ConfirmPassword { get; set; }

        public virtual string PhoneNumber { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual UsuarioIndicacaoViewModel UsuarioIndicacao { get; set; }
    }
}