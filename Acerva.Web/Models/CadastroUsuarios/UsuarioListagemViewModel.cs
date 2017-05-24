using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class UsuarioListagemViewModel
    {
        public virtual string Id { get; set; }
        public virtual string Matricula { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string NomeRegional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual string NomesPapeis { get; set; }
        public virtual bool EmailBoasVindasListaEnviado { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string PhoneNumber { get; set; }
    }
}