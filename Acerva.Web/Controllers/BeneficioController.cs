using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Extensions;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroBeneficios;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class BeneficioController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroBeneficios _cadastroBeneficios;
        private readonly IValidator<Beneficio> _validator;
        private readonly IIdentity _user;
        
        public BeneficioController(ICadastroBeneficios cadastroBeneficios, IValidator<Beneficio> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroBeneficios = cadastroBeneficios;
            _validator = validator;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaBeneficiosJson = _cadastroBeneficios.BuscaParaListagem()
                .Select(Mapper.Map<BeneficioViewModel>);
            return new JsonNetResult(listaBeneficiosJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return new JsonNetResult(new
            {
                
            });
        }

        public ActionResult Busca(int codigo)
        {
            var beneficio = _cadastroBeneficios.Busca(codigo);
            var beneficioJson = Mapper.Map<BeneficioViewModel>(beneficio);
            return new JsonNetResult(beneficioJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]BeneficioViewModel beneficioViewModel)
        {
            Log.InfoFormat("Usuário está salvando o benefício {0} de código {1}", beneficioViewModel.Nome, beneficioViewModel.Codigo);

            var ehNovo = beneficioViewModel.Codigo == 0;
            var beneficio = ehNovo ? new Beneficio() : _cadastroBeneficios.Busca(beneficioViewModel.Codigo);

            beneficioViewModel.Nome = beneficioViewModel.Nome.Trim();

            Mapper.Map(beneficioViewModel, beneficio);

            var validacao = _validator.Validate(beneficio);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(beneficio))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe umo benefício com o nome {0:unsafe}", beneficio.Nome));

            _cadastroBeneficios.Salva(beneficio);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Benefício <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), beneficio.Codigo, beneficio.Nome),
                "Benefício salva");

            return new JsonNetResult(new { growlMessage });
        }
        
        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando o benefício de id {2}", _user.Name, prefixoOperacao, id);

            var beneficio = _cadastroBeneficios.Busca(id);
            beneficio.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Benefício <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), beneficio.Codigo, beneficio.Nome, prefixoOperacao),
                string.Format("Benefício {0}ativada", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Benefício não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Beneficio beneficio)
        {
            var nomeUpper = beneficio.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroBeneficios
                .BuscaTodas()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != beneficio.Codigo);

            return temComMesmoNome;
        }
    }
}