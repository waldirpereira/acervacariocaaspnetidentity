using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class UsuarioViewModel
    {
        public virtual string Id { get; set; }
        public virtual string Matricula { get; set; }
        public virtual string Name { get; set; }
        public string FotoBase64 { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual UsuarioIndicacaoViewModel UsuarioIndicacao { get; set; }
    }
}