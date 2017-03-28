using System;

namespace Acerva.Web.Models.Acervo
{
    public class ArtigoListaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual CategoriaArtigoViewModel Categoria { get; set; }
        public virtual string NomeUsuario { get; set; }
    }
}