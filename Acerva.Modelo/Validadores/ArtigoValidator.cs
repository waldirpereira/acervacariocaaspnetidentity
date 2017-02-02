using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class ArtigoValidator : AbstractValidator<Artigo>
    {
        public ArtigoValidator()
        {
            RuleFor(e => e.Titulo)
                .NotEmpty()
                .Length(1, Artigo.TamanhoMaximoTitulo);

            RuleFor(e => e.TextoHtml)
                .NotEmpty()
                .Length(1, Artigo.TamanhoMaximoTextoHtml);
        }
    }
}
