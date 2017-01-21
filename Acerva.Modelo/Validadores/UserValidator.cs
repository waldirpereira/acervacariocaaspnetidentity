using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class UserValidator : AbstractValidator<Usuario>
    {
        public UserValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty();
        }
    }
}
