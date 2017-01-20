using System.Collections.Generic;
using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;

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

        public ActionResult BuscaNoticias()
        {
            var noticias = new List<string>
            {
                "noticia 1",
                "noticia 2"
            };
            return new JsonNetResult(noticias);
        }
    }
}