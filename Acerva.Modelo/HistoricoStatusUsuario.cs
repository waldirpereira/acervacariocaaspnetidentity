using System;

namespace Acerva.Modelo
{
    public class HistoricoStatusUsuario
    {
        public virtual int Codigo { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual StatusUsuario StatusNovo { get; set; }
        public virtual string IdUsuarioAlterado { get; set; }
        public virtual Usuario UsuarioLogado { get; set; }
        public virtual string EmailUsuarioLogado { get; set; }
    }
}
