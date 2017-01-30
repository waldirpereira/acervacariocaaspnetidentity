using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Web.Models.CadastroUsuarios;
using log4net;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class SituacaoController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroUsuarios _cadastroUsuarios;
        
        public SituacaoController(ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroUsuarios = cadastroUsuarios;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaSituacao(string termo)
        {
            var listaUsuariosJson = _cadastroUsuarios.BuscaComTermo(termo)
                .Select(Mapper.Map<UsuarioViewModel>);

            return new JsonNetResult(listaUsuariosJson);
        }
    }
}