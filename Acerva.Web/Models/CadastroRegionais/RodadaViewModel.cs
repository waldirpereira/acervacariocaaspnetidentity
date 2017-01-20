using System.Collections.Generic;

namespace Acerva.Web.Models.CadastroRegionais
{
    public class RodadaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual int Ordem { get; set; }
        public virtual ICollection<PartidaViewModel> Partidas { get; set; }
    }
}