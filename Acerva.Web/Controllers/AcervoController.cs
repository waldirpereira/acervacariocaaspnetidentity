using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models.Acervo;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class AcervoController : ApplicationBaseController
    {
        private readonly ICadastroArtigos _cadastroArtigos;

        public AcervoController(ICadastroArtigos cadastroArtigos, ICadastroUsuarios cadastroUsuarios) 
            : base(cadastroUsuarios)
        {
            _cadastroArtigos = cadastroArtigos;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaCategorias()
        {
            var usuarioLogado = HttpContext.User;
            
            var listaCategoriasJson = _cadastroArtigos.BuscaCategorias()
                .Where(c => c.Ativo && c.Artigos.Any(a => a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated)))
                .Select(c =>
                {
                    var categoriaJson = Mapper.Map<CategoriaArtigoViewModel>(c);
                    categoriaJson.QtdArtigos = c.Artigos
                        .Count(a => a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated));
                    return categoriaJson;
                });

            return new JsonNetResult(listaCategoriasJson);
        }

        public ActionResult BuscaArtigosDaCategoria(int codigoCategoriaArtigo)
        {
            var usuarioLogado = HttpContext.User;

            var listaArtigosJson = _cadastroArtigos.BuscaTodos()
                .Where(a => a.Categoria.Codigo == codigoCategoriaArtigo &&
                            (a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated)))
                .Select(Mapper.Map<ArtigoListaViewModel>);
            return new JsonNetResult(listaArtigosJson);
        }
        
        public ActionResult Busca(int codigo)
        {
            var artigo = _cadastroArtigos.Busca(codigo);
            var artigoJson = Mapper.Map<ArtigoViewModel>(artigo);
            return new JsonNetResult(artigoJson);
        }
    }
}