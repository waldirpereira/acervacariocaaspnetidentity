using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class BeneficioValidator : AbstractValidator<Beneficio>
    {
        public BeneficioValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Beneficio.TamanhoMaximoNome);
        }
    }
}
