using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroUsuarios;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    public class UsuarioController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IValidator<Usuario> _validator;
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly ICadastroRegionais _cadastroRegionais;

        public UsuarioController(IValidator<Usuario> validator, 
            ICadastroUsuarios cadastroUsuarios, ICadastroRegionais cadastroRegionais) : base(cadastroUsuarios)
        {
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _cadastroRegionais = cadastroRegionais;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaUsuariosJson = _cadastroUsuarios.BuscaParaListagem()
                .Select(Mapper.Map<UsuarioViewModel>);
            return new JsonNetResult(listaUsuariosJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            var regionaisJson = _cadastroRegionais.BuscaTodos()
                .Select(Mapper.Map<RegionalViewModel>);

            return new JsonNetResult(new
            {
                Regionais = regionaisJson
            });
        }

        public ActionResult Busca(string id)
        {
            var user = _cadastroUsuarios.Busca(id);
            var userJson = Mapper.Map<UsuarioViewModel>(user);
            return new JsonNetResult(userJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN, DIRETOR, DELEGADO")]
        public ActionResult Salva([JsonBinder]UsuarioViewModel usuarioViewModel)
        {
            Log.InfoFormat("Usuário está salvando o usuário {0} de código {1} e email {2}", 
                usuarioViewModel.Name, usuarioViewModel.Id, usuarioViewModel.Email);

            var ehNovo = string.IsNullOrEmpty(usuarioViewModel.Id);
            var usuario = ehNovo ? new Usuario{ Id = Guid.NewGuid().ToString() } : _cadastroUsuarios.Busca(usuarioViewModel.Id);

            usuarioViewModel.Name = usuarioViewModel.Name.Trim();

            Mapper.Map(usuarioViewModel, usuario);

            var validacao = _validator.Validate(usuario);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (_cadastroUsuarios.ExisteComMesmoNome(usuario))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um usuário com o nome {0:unsafe}", usuario.Name));
            
            if (ehNovo)
                _cadastroUsuarios.SalvaNovo(usuario);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Usuário <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), usuario.Id, usuario.Name),
                "Usuário salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraStatus(int id, StatusUsuario status)
        {
            //var prefixoOperacao = ativo ? string.Empty : "des";
            //Log.InfoFormat("Usuário {0} está {1}atividando a User de id {2}", _user.Name, prefixoOperacao, id);

            //var User = _cadastroRegionais.Busca(id);
            //User.Ativo = ativo;

            //var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
            //    string.Format("User <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), User.Codigo, User.Codigo, prefixoOperacao),
            //    string.Format("User {0}ativado", prefixoOperacao));

            //return new JsonNetResult(new { growlMessage });

            return null;
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Usuário não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }
        
        public ActionResult BuscaUsuariosAtivosComTermo(string termo)
        {
            var usuariosDisponiveis = _cadastroUsuarios.BuscaComTermo(termo)
                .OrderBy(p => p.Name)
                .Take(20)
                .Select(p => new
                {
                    p.Id,
                    p.Name
                });

            return new JsonNetResult(usuariosDisponiveis);
        }
    }
}