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
        public virtual string Cpf { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string SiglaUf { get; set; }
        public virtual string Cep { get; set; }
        public virtual string CodigoBdSexo { get; set; }
    }
}