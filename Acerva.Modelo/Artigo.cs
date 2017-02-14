using System;
using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Artigo
    {
        public const int TamanhoMaximoTitulo = 300;
        public const int TamanhoMaximoTextoHtml = 8000;

        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual CategoriaArtigo Categoria { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual VisibilidadeArtigo Visibilidade { get; set; }
        public virtual bool Ativo { get; set; }
        private IEnumerable<AnexoArtigo> _anexos = new List<AnexoArtigo>();

        public virtual IEnumerable<AnexoArtigo> Anexos
        {
            get { return _anexos; }
            set { _anexos = value; }
        }
    }
}
