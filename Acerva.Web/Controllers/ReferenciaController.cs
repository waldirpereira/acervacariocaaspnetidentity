using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Models.Referencia;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class ReferenciaController : ApplicationBaseController
    {
        private readonly ICadastroArtigos _cadastroArtigos;

        public ReferenciaController(ICadastroArtigos cadastroArtigos, ICadastroUsuarios cadastroUsuarios) 
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
            var listaCategoriasJson = _cadastroArtigos.BuscaCategorias()
                .Where(c => c.Ativo && c.Artigos.Any())
                .Select(Mapper.Map<CategoriaArtigoViewModel>);
            return new JsonNetResult(listaCategoriasJson);
        }

        public ActionResult BuscaArtigosDaCategoria(int codigoCategoriaArtigo)
        {
            var listaArtigosJson = _cadastroArtigos.BuscaTodos()
                .Where(a => a.Categoria.Codigo == codigoCategoriaArtigo)
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