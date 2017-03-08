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
using Acerva.Web.Models.CadastroAvioes;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class AviaoController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroAvioes _cadastroAvioes;
        private readonly IValidator<Aviao> _validator;
        private readonly IIdentity _user;
        
        public AviaoController(ICadastroAvioes cadastroAvioes, IValidator<Aviao> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroAvioes = cadastroAvioes;
            _validator = validator;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaAvioesJson = _cadastroAvioes.BuscaParaListagem()
                .Select(Mapper.Map<AviaoViewModel>);
            return new JsonNetResult(listaAvioesJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return new JsonNetResult(new
            {
                
            });
        }

        public ActionResult Busca(int codigo)
        {
            var aviao = _cadastroAvioes.Busca(codigo);
            var aviaoJson = Mapper.Map<AviaoViewModel>(aviao);
            return new JsonNetResult(aviaoJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]AviaoViewModel aviaoViewModel)
        {
            Log.InfoFormat("Usuário está salvando %ARTIGO% avião {0} de código {1}", aviaoViewModel.Nome, aviaoViewModel.Codigo);

            var ehNovo = aviaoViewModel.Codigo == 0;
            var aviao = ehNovo ? new Aviao() : _cadastroAvioes.Busca(aviaoViewModel.Codigo);

            aviaoViewModel.Nome = aviaoViewModel.Nome.Trim();

            Mapper.Map(aviaoViewModel, aviao);

            var validacao = _validator.Validate(aviao);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(aviao))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um%ARTIGO% avião com o nome {0:unsafe}", aviao.Nome));

            _cadastroAvioes.Salva(aviao);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Avião <a href='{0}#/Edit/{1}'>{2}</a> foi salv%ARTIGO% com sucesso", Url.Action("Index"), aviao.Codigo, aviao.Nome),
                "Avião salva");

            return new JsonNetResult(new { growlMessage });
        }
        
        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando %ARTIGO% avião de id {2}", _user.Name, prefixoOperacao, id);

            var aviao = _cadastroAvioes.Busca(id);
            aviao.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Avião <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativad%ARTIGO% com sucesso", Url.Action("Index"), aviao.Codigo, aviao.Nome, prefixoOperacao),
                string.Format("Avião {0}ativada", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Avião não salv%ARTIGO%");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Aviao aviao)
        {
            var nomeUpper = aviao.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroAvioes
                .BuscaTodas()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != aviao.Codigo);

            return temComMesmoNome;
        }
    }
}