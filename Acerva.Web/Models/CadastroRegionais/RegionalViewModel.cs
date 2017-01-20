using System.Collections.Generic;

namespace Acerva.Web.Models.CadastroRegionais
{
    public class RegionalViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual ICollection<RodadaViewModel> Rodadas { get; set; }
        public virtual ICollection<EquipeViewModel> Equipes { get; set; }
    }
}