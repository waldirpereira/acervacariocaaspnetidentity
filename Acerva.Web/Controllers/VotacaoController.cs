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
using Acerva.Web.Models.CadastroVotacoes;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class VotacaoController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroVotacoes _cadastroVotacoes;
        private readonly IValidator<Votacao> _validator;
        private readonly IIdentity _user;
        
        public VotacaoController(ICadastroVotacoes cadastroVotacoes, IValidator<Votacao> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroVotacoes = cadastroVotacoes;
            _validator = validator;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaVotacoesJson = _cadastroVotacoes.BuscaParaListagem()
                .Select(Mapper.Map<VotacaoViewModel>);
            return new JsonNetResult(listaVotacoesJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return new JsonNetResult(new
            {
                
            });
        }

        public ActionResult Busca(int codigo)
        {
            var votacao = _cadastroVotacoes.Busca(codigo);
            var votacaoJson = Mapper.Map<VotacaoViewModel>(votacao);
            return new JsonNetResult(votacaoJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]VotacaoViewModel votacaoViewModel)
        {
            Log.InfoFormat("Usuário está salvando a votação {0} de código {1}", votacaoViewModel.Nome, votacaoViewModel.Codigo);

            var ehNovo = votacaoViewModel.Codigo == 0;
            var votacao = ehNovo ? new Votacao() : _cadastroVotacoes.Busca(votacaoViewModel.Codigo);

            votacaoViewModel.Nome = votacaoViewModel.Nome.Trim();

            Mapper.Map(votacaoViewModel, votacao);

            var validacao = _validator.Validate(votacao);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(votacao))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma votação com o nome {0:unsafe}", votacao.Nome));

            _cadastroVotacoes.Salva(votacao);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Votação <a href='{0}#/Edit/{1}'>{2}</a> foi salva com sucesso", Url.Action("Index"), votacao.Codigo, votacao.Nome),
                "Votação salva");

            return new JsonNetResult(new { growlMessage });
        }
        
        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a votação de id {2}", _user.Name, prefixoOperacao, id);

            var votacao = _cadastroVotacoes.Busca(id);
            votacao.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Votação <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), votacao.Codigo, votacao.Nome, prefixoOperacao),
                string.Format("Votação {0}ativada", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Votação não salva");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Votacao votacao)
        {
            var nomeUpper = votacao.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroVotacoes
                .BuscaTodas()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != votacao.Codigo);

            return temComMesmoNome;
        }
    }
}