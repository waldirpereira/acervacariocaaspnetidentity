using System;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroArtigos
{
    public class ArtigoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual VisibilidadeArtigo Visibilidade { get; set; }
        public virtual CategoriaArtigoViewModel Categoria { get; set; }
        public virtual UsuarioViewModel Usuario { get; set; }
        public virtual bool Ativo { get; set; }
    }
}