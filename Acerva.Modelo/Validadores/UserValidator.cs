using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class UserValidator : AbstractValidator<IdentityUser>
    {
        public UserValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty();
        }
    }
}
