using System.Web.Mvc;
using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        public HomeController(ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios) {}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Em nosso site você poderá criar e participar de ACervAs de uma maneira totalmente simplificada e segura.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Aqui você encontra nossos contatos.";

            return View();
        }

        public ActionResult ConfirmSent()
        {
            return View("ConfirmSent");
        }
    }
}