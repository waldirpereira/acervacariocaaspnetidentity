using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class AviaoValidator : AbstractValidator<Aviao>
    {
        public AviaoValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Aviao.TamanhoMaximoNome);
        }
    }
}
