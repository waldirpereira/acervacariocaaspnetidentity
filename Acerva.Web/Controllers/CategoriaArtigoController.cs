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
using Acerva.Web.Models.CadastroCategoriasArtigos;
using FluentValidation;
using log4net;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class CategoriaArtigoController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroCategoriasArtigos _cadastroCategoriasArtigos;
        private readonly IValidator<CategoriaArtigo> _validator;
        private readonly IIdentity _user;
        
        public CategoriaArtigoController(ICadastroCategoriasArtigos cadastroCategoriasArtigos, IValidator<CategoriaArtigo> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroCategoriasArtigos = cadastroCategoriasArtigos;
            _validator = validator;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaCategoriasArtigosJson = _cadastroCategoriasArtigos.BuscaParaListagem()
                .Select(Mapper.Map<CategoriaArtigoViewModel>);
            return new JsonNetResult(listaCategoriasArtigosJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            return new JsonNetResult(new
            {
                
            });
        }

        public ActionResult Busca(int codigo)
        {
            var categoriaArtigo = _cadastroCategoriasArtigos.Busca(codigo);
            var categoriaArtigoJson = Mapper.Map<CategoriaArtigoViewModel>(categoriaArtigo);
            return new JsonNetResult(categoriaArtigoJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]CategoriaArtigoViewModel categoriaArtigoViewModel)
        {
            Log.InfoFormat("Usuário está salvando a categoria de artigo {0} de código {1}", categoriaArtigoViewModel.Nome, categoriaArtigoViewModel.Codigo);

            var ehNovo = categoriaArtigoViewModel.Codigo == 0;
            var categoriaArtigo = ehNovo ? new CategoriaArtigo() : _cadastroCategoriasArtigos.Busca(categoriaArtigoViewModel.Codigo);

            categoriaArtigoViewModel.Nome = categoriaArtigoViewModel.Nome.Trim();

            Mapper.Map(categoriaArtigoViewModel, categoriaArtigo);

            var validacao = _validator.Validate(categoriaArtigo);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoNome(categoriaArtigo))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma categoria de artigo com o nome {0:unsafe}", categoriaArtigo.Nome));

            _cadastroCategoriasArtigos.Salva(categoriaArtigo);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Categoria de artigo <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), categoriaArtigo.Codigo, categoriaArtigo.Nome),
                "Categoria de artigo salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a categoria de artigo de id {2}", _user.Name, prefixoOperacao, id);

            var categoriaArtigo = _cadastroCategoriasArtigos.Busca(id);
            categoriaArtigo.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Categoria de artigo <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), categoriaArtigo.Codigo, categoriaArtigo.Codigo, prefixoOperacao),
                string.Format("Categoria de artigo {0}ativado", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Categoria de artigo não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(CategoriaArtigo categoriaArtigo)
        {
            var nomeUpper = categoriaArtigo.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroCategoriasArtigos
                .BuscaTodos()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != categoriaArtigo.Codigo);

            return temComMesmoNome;
        }
    }
}