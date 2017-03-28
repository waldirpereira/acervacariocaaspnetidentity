using System.Collections.Generic;

namespace Acerva.Web.Models.Estatuto
{
    public class ArtigoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual IEnumerable<AnexoArtigoViewModel> Anexos { get; set; }
    }
}