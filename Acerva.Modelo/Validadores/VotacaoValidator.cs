using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class VotacaoValidator : AbstractValidator<Votacao>
    {
        public VotacaoValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Votacao.TamanhoMaximoNome);
        }
    }
}
