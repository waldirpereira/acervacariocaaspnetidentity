using System.Web.Mvc;
using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers
{
    public class ApplicationBaseController : Controller
    {
        protected readonly ICadastroUsuarios CadastroUsuarios;

        public ApplicationBaseController(ICadastroUsuarios cadastroUsuarios)
        {
            CadastroUsuarios = cadastroUsuarios;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = CadastroUsuarios.BuscaPeloEmail(username);
                    ViewData.Add("Name", user.Name);
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public ApplicationBaseController()
        { }
    }
}