using FluentValidation;

namespace Acerva.Modelo.Validadores
{
    public class NoticiaValidator : AbstractValidator<Noticia>
    {
        public NoticiaValidator()
        {
            RuleFor(e => e.Titulo)
                .NotEmpty()
                .Length(1, Noticia.TamanhoMaximoTitulo);

            RuleFor(e => e.TextoHtml)
                .NotEmpty()
                .Length(1, Noticia.TamanhoMaximoTextoHtml);
        }
    }
}
