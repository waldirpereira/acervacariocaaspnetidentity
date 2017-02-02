using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Models.CadastroUsuarios;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class SituacaoController : ApplicationBaseController
    {
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly UsuarioControllerHelper _usuarioControllerHelper;

        public SituacaoController(ICadastroUsuarios cadastroUsuarios, UsuarioControllerHelper usuarioControllerHelper) : base(cadastroUsuarios)
        {
            _cadastroUsuarios = cadastroUsuarios;
            _usuarioControllerHelper = usuarioControllerHelper;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaSituacao(string cpf)
        {
            var user = _cadastroUsuarios.BuscaPeloCpf(cpf);
            var userJson = Mapper.Map<UsuarioViewModel>(user);

            userJson.FotoBase64 = _usuarioControllerHelper.BuscaFotoBase64(user.Id, HttpContext);

            return new JsonNetResult(userJson);
        }
    }
}