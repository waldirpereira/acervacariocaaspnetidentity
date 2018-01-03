using System.Web.Mvc;
using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers
{
    public class LojaController : ApplicationBaseController
    {
        public LojaController(ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}