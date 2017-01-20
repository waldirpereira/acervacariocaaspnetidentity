using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class AcervaValidator : AbstractValidator<Acerva>
    {
        public AcervaValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Acerva.TamanhoMaximoNome);
        }
    }
}
