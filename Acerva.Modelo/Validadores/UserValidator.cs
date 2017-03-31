using System.Text.RegularExpressions;
using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class UserValidator : AbstractValidator<Usuario>
    {
        public UserValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty();

            RuleFor(u => u.Email)
                .Matches(new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }
    }
}
