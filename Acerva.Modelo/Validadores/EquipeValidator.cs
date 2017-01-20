using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class EquipeValidator : AbstractValidator<Equipe>
    {
        public EquipeValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, Equipe.TamanhoMaximoNome);

            RuleFor(e => e.Sigla)
                .NotEmpty()
                .Length(1, Equipe.TamanhoMaximoSigla);
        }
    }
}
