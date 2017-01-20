using System.Collections.Generic;

namespace Acerva.Web.Models.CadastroPalpites
{
    public class AcervaViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual IdentityUserViewModel UsuarioResponsavel { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual ICollection<ParticipacaoViewModel> Participacoes { get; set; }
    }
}