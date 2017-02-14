using System.Linq;
using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models.Home;
using AutoMapper;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly ICadastroNoticias _cadastroNoticias;
        public HomeController(ICadastroUsuarios cadastroUsuarios, ICadastroNoticias cadastroNoticias) : base(cadastroUsuarios)
        {
            _cadastroUsuarios = cadastroUsuarios;
            _cadastroNoticias = cadastroNoticias;
        }

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

        public ActionResult IndicacoesAConfirmar()
        {
            ViewBag.Message = "Esta é a lista de pessoas que disseram ter sido indicadas por você e que aguardam sua confirmação.";

            return View("IndicacoesAConfirmar");
        }


        public ActionResult BuscaNoticias()
        {
            var listaNoticiasJson = _cadastroNoticias.BuscaTodas()
                .Where(n => n.Ativo)
                .OrderBy(n => n.Ordem.HasValue ? n.Ordem.Value : int.MaxValue)
                .ThenBy(n => n.Codigo)
                .Select(Mapper.Map<NoticiaViewModel>);
            return new JsonNetResult(listaNoticiasJson);
        }

        public ActionResult BuscaIndicacoesAConfirmar()
        {
            var usuarioLogado = HttpContext.User.Identity;
            var listaUsuariosIndicados = _cadastroUsuarios.BuscaUsuariosIndicados(usuarioLogado.GetUserId())
                .Where(u => u.Status == StatusUsuario.AguardandoIndicacao)
                .Select(Mapper.Map<UsuarioIndicacaoViewModel>);

            return new JsonNetResult(listaUsuariosIndicados);
        }
    }
}