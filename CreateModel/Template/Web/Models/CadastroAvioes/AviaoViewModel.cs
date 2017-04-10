using System;

namespace Acerva.Web.Models.CadastroAvioes
{
    public class AviaoViewModel
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }
    }
}