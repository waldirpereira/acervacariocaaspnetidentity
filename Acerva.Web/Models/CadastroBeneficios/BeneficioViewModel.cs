using System;

namespace Acerva.Web.Models.CadastroBeneficios
{
    public class BeneficioViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }
    }
}