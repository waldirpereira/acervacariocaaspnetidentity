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
using Acerva.Web.Models.CadastroNoticias;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class NoticiaController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroNoticias _cadastroNoticias;
        private readonly IValidator<Noticia> _validator;
        private readonly IIdentity _user;
        
        public NoticiaController(ICadastroNoticias cadastroNoticias, IValidator<Noticia> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroNoticias = cadastroNoticias;
            _validator = validator;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaNoticiasJson = _cadastroNoticias.BuscaParaListagem()
                .Select(Mapper.Map<NoticiaViewModel>);
            return new JsonNetResult(listaNoticiasJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return new JsonNetResult(new
            {
                
            });
        }

        public ActionResult Busca(int codigo)
        {
            var noticia = _cadastroNoticias.Busca(codigo);
            var noticiaJson = Mapper.Map<NoticiaViewModel>(noticia);
            return new JsonNetResult(noticiaJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]NoticiaViewModel noticiaViewModel)
        {
            Log.InfoFormat("Usuário está salvando a notícia {0} de código {1}", noticiaViewModel.Titulo, noticiaViewModel.Codigo);

            var ehNovo = noticiaViewModel.Codigo == 0;
            var noticia = ehNovo ? new Noticia() : _cadastroNoticias.Busca(noticiaViewModel.Codigo);

            noticiaViewModel.Titulo = noticiaViewModel.Titulo.Trim();

            Mapper.Map(noticiaViewModel, noticia);

            var validacao = _validator.Validate(noticia);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(noticia))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma notícia com o nome {0:unsafe}", noticia.Titulo));

            _cadastroNoticias.Salva(noticia);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Notícia <a href='{0}#/Edit/{1}'>{2}</a> foi salva com sucesso", Url.Action("Index"), noticia.Codigo, noticia.Titulo),
                "Notícia salva");

            return new JsonNetResult(new { growlMessage });
        }
        
        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a notícia de id {2}", _user.Name, prefixoOperacao, id);

            var noticia = _cadastroNoticias.Busca(id);
            noticia.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Notícia <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), noticia.Codigo, noticia.Titulo, prefixoOperacao),
                string.Format("Notícia {0}ativada", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Notícia não salva");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Noticia noticia)
        {
            var nomeUpper = noticia.Titulo.ToUpperInvariant();
            var temComMesmoNome = _cadastroNoticias
                .BuscaTodas()
                .Any(e => e.Titulo.ToUpperInvariant() == nomeUpper && e.Codigo != noticia.Codigo);

            return temComMesmoNome;
        }
    }
}