using System;
using System.Collections.Generic;

namespace Acerva.Web.Models.Referencia
{
    public class ArtigoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual CategoriaArtigoViewModel Categoria { get; set; }
        public virtual string NomeUsuario { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual IEnumerable<AnexoArtigoViewModel> Anexos { get; set; }
    }
}