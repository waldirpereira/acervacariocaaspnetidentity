using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Models.CadastroRegionais;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class ListaRegionaisController : ApplicationBaseController
    {
        private readonly ICadastroRegionais _cadastroRegionais;
        
        public ListaRegionaisController(ICadastroRegionais cadastroRegionais, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroRegionais = cadastroRegionais;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaRegionaisJson = _cadastroRegionais.BuscaParaListagem()
                .Where(r => r.Ativo)
                .Select(Mapper.Map<RegionalViewModel>);
            return new JsonNetResult(listaRegionaisJson);
        }
    }
}