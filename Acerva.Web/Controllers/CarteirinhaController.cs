using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Models.CadastroUsuarios;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    [Authorize]
    public class CarteirinhaController : ApplicationBaseController
    {
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly UsuarioControllerHelper _usuarioControllerHelper;

        public CarteirinhaController(ICadastroUsuarios cadastroUsuarios, UsuarioControllerHelper usuarioControllerHelper) : base(cadastroUsuarios)
        {
            _cadastroUsuarios = cadastroUsuarios;
            _usuarioControllerHelper = usuarioControllerHelper;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Busca()
        {
            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;

            var userJson = Mapper.Map<UsuarioViewModel>(usuarioLogadoBd);

            if (usuarioLogadoBd != null)
                userJson.FotoBase64 = _usuarioControllerHelper.BuscaFotoBase64(usuarioLogadoBd.Id, HttpContext, true);

            return new JsonNetResult(userJson);
        }
    }
}