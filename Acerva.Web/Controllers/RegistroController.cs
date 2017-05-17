using System.Web.Mvc;
using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers
{
    public class RegistroController : ApplicationBaseController
    {
        public RegistroController(ICadastroUsuarios cadastroUsuarios)
            : base(cadastroUsuarios)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Register", "Account");
        }
    }
}