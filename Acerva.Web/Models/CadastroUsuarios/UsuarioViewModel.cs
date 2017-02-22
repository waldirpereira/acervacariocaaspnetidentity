using System;
using System.Collections.Generic;
using Acerva.Modelo;

namespace Acerva.Web.Models.CadastroUsuarios
{
    public class UsuarioViewModel
    {
        public virtual string Id { get; set; }
        public virtual string Matricula { get; set; }
        public virtual string Name { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string TelefoneFixo { get; set; }
        public virtual string Rg { get; set; }
        public virtual string Cpf { get; set; }
        public string FotoBase64 { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual UsuarioIndicacaoViewModel UsuarioIndicacao { get; set; }
        private IEnumerable<PapelViewModel> _papeis = new List<PapelViewModel>();
        public virtual IEnumerable<PapelViewModel> Papeis { get {return _papeis; } set { _papeis = value; } }
        public virtual string NomesPapeis { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual UfViewModel Uf { get; set; }
        public virtual string Cep { get; set; }
        public virtual DateTime? DataNascimento { get; set; }
        public virtual DateTime? DataAdmissao { get; set; }
        public virtual string EmailLista { get; set; }
        public virtual Sexo? Sexo { get; set; }
        public virtual string Experiencia { get; set; }
        public virtual string Observacao { get; set; }
        public virtual bool EmailBoasVindasListaEnviado { get; set; }
    }
}