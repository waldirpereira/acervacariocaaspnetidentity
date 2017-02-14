using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class CategoriaArtigoValidator : AbstractValidator<CategoriaArtigo>
    {
        public CategoriaArtigoValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty()
                .Length(1, CategoriaArtigo.TamanhoMaximoNome);
        }
    }
}
