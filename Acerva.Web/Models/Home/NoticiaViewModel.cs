using System;

namespace Acerva.Web.Models.Home
{
    public class NoticiaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual DateTime DataInicio { get; set; }
        public virtual DateTime? DataFim { get; set; }
        public virtual int? Ordem { get; set; }
    }
}