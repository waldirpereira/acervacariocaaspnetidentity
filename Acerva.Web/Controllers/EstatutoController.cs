using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models.Acervo;
using AutoMapper;

namespace Acerva.Web.Controllers
{
    public class EstatutoController : ApplicationBaseController
    {
        private readonly ICadastroArtigos _cadastroArtigos;

        public EstatutoController(ICadastroArtigos cadastroArtigos, ICadastroUsuarios cadastroUsuarios)
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
            var artigoEstatuto = _cadastroArtigos.Busca(Artigo.CodigoArtigoEstatuto);
            var artigoJson = Mapper.Map<ArtigoViewModel>(artigoEstatuto);
            return new JsonNetResult(artigoJson);
        }
    }
}