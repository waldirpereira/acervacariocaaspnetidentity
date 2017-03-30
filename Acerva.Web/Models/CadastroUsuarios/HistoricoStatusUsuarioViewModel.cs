using System;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class HistoricoStatusUsuarioViewModel
    {
        public virtual DateTime DataHora { get; set; }
        public virtual StatusUsuario StatusNovo { get; set; }
        public virtual string IdUsuarioAlterado { get; set; }
        public virtual string NomeUsuarioLogado { get; set; }
    }
}