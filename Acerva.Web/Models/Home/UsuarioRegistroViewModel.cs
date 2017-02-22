using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Acerva.Modelo;

namespace Acerva.Web.Models.Home
{
    public class UsuarioRegistroViewModel
    {
        public virtual string Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string FotoBase64 { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual RegionalViewModel Regional { get; set; }
        public virtual StatusUsuario Status { get; set; }
        public virtual UsuarioIndicacaoViewModel UsuarioIndicacao { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual UfViewModel Uf { get; set; }
        public virtual string Cep { get; set; }
        public virtual string TelefoneFixo { get; set; }
        public virtual string Rg { get; set; }
        public virtual DateTime? DataNascimento { get; set; }
        public virtual Sexo? Sexo { get; set; }
        public virtual string Experiencia { get; set; }
    }
}