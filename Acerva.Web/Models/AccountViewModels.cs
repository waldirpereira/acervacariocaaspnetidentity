using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acerva.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = @"E-mail")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = @"Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = @"Lembrar este browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = @"E-mail")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = @"E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"Senha")]
        public string Password { get; set; }

        [Display(Name = @"Lembrar de mim?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
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
    }

    public class ResetPasswordViewModel
    {
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

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = @"E-mail")]
        public string Email { get; set; }
    }
}
