using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class RegionalValidator : AbstractValidator<Regional>
    {
        public RegionalValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Regional.TamanhoMaximoNome);
        }
    }
}
