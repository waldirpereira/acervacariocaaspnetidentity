using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Controllers.Helpers;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroRegionais;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN")]
    public class RegionalController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroRegionais _cadastroRegionais;
        private readonly IValidator<Regional> _validator;
        private readonly RegionalControllerHelper _helper;
        private readonly IIdentity _user;
        
        public RegionalController(ICadastroRegionais cadastroRegionais, IValidator<Regional> validator, IPrincipal user, ICadastroUsuarios cadastroUsuarios, RegionalControllerHelper helper) : base(cadastroUsuarios)
        {
            _cadastroRegionais = cadastroRegionais;
            _validator = validator;
            _helper = helper;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaRegionaisJson = _cadastroRegionais.BuscaParaListagem()
                .Select(Mapper.Map<RegionalViewModel>);
            return new JsonNetResult(listaRegionaisJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            var equipesJson = _cadastroRegionais.BuscaEquipes()
                .Select(Mapper.Map<EquipeViewModel>);

            return new JsonNetResult(new
            {
                Equipes = equipesJson
            });
        }

        public ActionResult Busca(int codigo)
        {
            var regional = _cadastroRegionais.Busca(codigo);
            var regionalJson = Mapper.Map<RegionalViewModel>(regional);
            return new JsonNetResult(regionalJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]RegionalViewModel regionalViewModel)
        {
            Log.InfoFormat("Usuário está salvando a regional {0} de código {1}", regionalViewModel.Nome, regionalViewModel.Codigo);

            var ehNovo = regionalViewModel.Codigo == 0;
            var regional = ehNovo ? new Regional() : _cadastroRegionais.Busca(regionalViewModel.Codigo);

            regionalViewModel.Nome = regionalViewModel.Nome.Trim();

            var partidasTerminadasNestaAtualizacao = _helper.PegaPartidasTerminadasNestaAtualizacao(regionalViewModel, regional);
            _helper.CalculaPontuacoesParaPartidasTerminadas(partidasTerminadasNestaAtualizacao);

            Mapper.Map(regionalViewModel, regional);

            var validacao = _validator.Validate(regional);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(regional))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma regional com o nome {0:unsafe}", regional.Nome));

            _cadastroRegionais.Salva(regional);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Regional <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), regional.Codigo, regional.Nome),
                "Regional salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a regional de id {2}", _user.Name, prefixoOperacao, id);

            var regional = _cadastroRegionais.Busca(id);
            regional.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Regional <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), regional.Codigo, regional.Codigo, prefixoOperacao),
                string.Format("Regional {0}ativado", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Regional não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Regional regional)
        {
            var nomeUpper = regional.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroRegionais
                .BuscaTodos()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != regional.Codigo);

            return temComMesmoNome;
        }

        public ActionResult BuscaPlacares(int codigoRodada)
        {
            var rodada = _cadastroRegionais.BuscaRodada(codigoRodada);
            if (rodada == null)
                return null;

            var controller = DependencyResolver.Current.GetService<ImportacaoController>();

            controller.BuscaAtualizacoesPartidasDaRodada(rodada);

            var partidasJson = rodada.Partidas.Select(Mapper.Map<PartidaViewModel>);
            return new JsonNetResult(partidasJson);
        }
    }
}