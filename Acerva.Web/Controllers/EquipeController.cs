using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroEquipes;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN")]
    public class EquipeController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroEquipes _cadastroEquipes;
        private readonly IValidator<Equipe> _validator;

        public EquipeController(ICadastroEquipes cadastroEquipes, IValidator<Equipe> validator, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroEquipes = cadastroEquipes;
            _validator = validator;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaEquipesJson = _cadastroEquipes.BuscaParaListagem()
                .Select(Mapper.Map<EquipeViewModel>);
            return new JsonNetResult(listaEquipesJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return null;
        }

        public ActionResult Busca(int codigo)
        {
            var equipe = _cadastroEquipes.Busca(codigo);
            var equipeJson = Mapper.Map<EquipeViewModel>(equipe);
            return new JsonNetResult(equipeJson);
        }

        [Transacao]
        public ActionResult Salva([JsonBinder]EquipeViewModel equipeViewModel)
        {
            Log.InfoFormat("Usuário está salvando a equipe {0} - {1} de código {2}", equipeViewModel.Sigla, equipeViewModel.Nome, equipeViewModel.Codigo);

            var ehNova = equipeViewModel.Codigo == 0;
            var equipe = ehNova ? new Equipe() : _cadastroEquipes.Busca(equipeViewModel.Codigo);

            equipeViewModel.Nome = equipeViewModel.Nome.Trim();

            Mapper.Map(equipeViewModel, equipe);

            var validacao = _validator.Validate(equipe);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());
            
            if (ExisteComMesmoNome(equipe))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma equipe com o nome {0:unsafe}", equipe.Nome));

            if (ehNova)
            {
                equipe.Codigo = _cadastroEquipes.BuscaProximoCodigo();
                _cadastroEquipes.Salva(equipe);
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Equipe <a href='{0}#/Edit/{1}'>{2}</a> foi salva com sucesso", Url.Action("Index"), equipe.Codigo, equipe.Nome),
                "Equipe salva");

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Equipe não salva");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Equipe equipe)
        {
            var nomeUpper = equipe.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroEquipes
                .BuscaTodas()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != equipe.Codigo);

            return temComMesmoNome;
        }
    }
}