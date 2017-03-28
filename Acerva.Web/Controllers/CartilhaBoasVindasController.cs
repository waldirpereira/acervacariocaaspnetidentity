using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models.Acervo;
using AutoMapper;

namespace Acerva.Web.Controllers
{
    public class CartilhaBoasVindasController : ApplicationBaseController
    {
        private readonly ICadastroArtigos _cadastroArtigos;

        public CartilhaBoasVindasController(ICadastroArtigos cadastroArtigos, ICadastroUsuarios cadastroUsuarios)
            : base(cadastroUsuarios)
        {
            _cadastroArtigos = cadastroArtigos;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Busca()
        {
            var artigoEstatuto = _cadastroArtigos.Busca(Artigo.CodigoArtigoBoasVindas);
            var artigoJson = Mapper.Map<ArtigoViewModel>(artigoEstatuto);
            artigoJson.TextoHtml = artigoJson.TextoHtml.Replace(" %NOME%", string.Empty);
            return new JsonNetResult(artigoJson);
        }
    }
}