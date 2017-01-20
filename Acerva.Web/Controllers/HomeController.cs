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
            ViewBag.Message = "A ACervA Carioca é uma associação que visa incentivar o desenvolvimento da cultura da cerveja artesanal, no Rio de Janeiro e em todo o Brasil, promovendo encontros, palestras, cursos, concursos e degustações das mais variadas cervejas, em grande parte produzidas pelos próprios associados. Descubra e se delicie com essa cultura participando da associação e dos nossos encontros. E aprenda também a produzir cervejas diferenciadas e de alta qualidade.";

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