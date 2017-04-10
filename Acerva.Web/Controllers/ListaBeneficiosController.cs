using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Models.CadastroBeneficios;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class ListaBeneficiosController : ApplicationBaseController
    {
        private readonly ICadastroBeneficios _cadastroBeneficios;
        
        public ListaBeneficiosController(ICadastroBeneficios cadastroBeneficios, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroBeneficios = cadastroBeneficios;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaBeneficiosJson = _cadastroBeneficios.BuscaParaListagem()
                .Where(b => b.Ativo)
                .Select(Mapper.Map<BeneficioViewModel>);
            return new JsonNetResult(listaBeneficiosJson);
        }
    }
}