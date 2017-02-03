using System.Web.Mvc;
using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers
{
    public class AgendaController : ApplicationBaseController
    {
        public AgendaController(ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}