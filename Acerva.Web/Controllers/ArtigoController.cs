using System;
using System.IO;
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
using Acerva.Web.Models.CadastroArtigos;
using FluentValidation;
using log4net;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN, DIRETOR")]
    public class ArtigoController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICadastroArtigos _cadastroArtigos;
        private readonly IValidator<Artigo> _validator;
        private readonly ICadastroUsuarios _cadastroUsuarios;
        private readonly IIdentity _user;
        
        public ArtigoController(ICadastroArtigos cadastroArtigos, IValidator<Artigo> validator, 
            IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroArtigos = cadastroArtigos;
            _validator = validator;
            _cadastroUsuarios = cadastroUsuarios;
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaParaListagem()
        {
            var listaArtigosJson = _cadastroArtigos.BuscaParaListagem()
                .Select(Mapper.Map<ArtigoViewModel>);
            return new JsonNetResult(listaArtigosJson);
        }

        public ActionResult BuscaAnexos(int codigoArtigo)
        {
            var artigo = _cadastroArtigos.Busca(codigoArtigo);

            var listaAnexosJson = artigo.Anexos
                .Select(Mapper.Map<AnexoArtigoViewModel>);
            return new JsonNetResult(listaAnexosJson);
        }

        public ActionResult BuscaTiposDominio()
        {
            var categoriasJson = _cadastroArtigos.BuscaCategorias()
                .Select(Mapper.Map<CategoriaArtigoViewModel>);

            return new JsonNetResult(new
            {
                Categorias = categoriasJson
            });
        }

        public ActionResult Busca(int codigo)
        {
            var artigo = _cadastroArtigos.Busca(codigo);
            var artigoJson = Mapper.Map<ArtigoViewModel>(artigo);
            return new JsonNetResult(artigoJson);
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult Salva([JsonBinder]ArtigoViewModel artigoViewModel)
        {
            Log.InfoFormat("Usuário está salvando a artigo {0} de código {1}", artigoViewModel.Titulo, artigoViewModel.Codigo);

            var ehNovo = artigoViewModel.Codigo == 0;
            var artigo = ehNovo ? new Artigo() : _cadastroArtigos.Busca(artigoViewModel.Codigo);

            artigoViewModel.Titulo = artigoViewModel.Titulo.Trim();

            Mapper.Map(artigoViewModel, artigo);

            var usuarioLogado = HttpContext.User;
            var usuarioLogadoBd = usuarioLogado.Identity.IsAuthenticated ? _cadastroUsuarios.Busca(usuarioLogado.Identity.GetUserId()) : null;
            artigo.Usuario = usuarioLogadoBd;

            var validacao = _validator.Validate(artigo);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            if (ExisteComMesmoTitulo(artigo))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma artigo com o nome {0:unsafe}", artigo.Titulo));

            artigo.DataHora = DateTime.Now;

            _cadastroArtigos.Salva(artigo);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Artigo <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", Url.Action("Index"), artigo.Codigo, artigo.Titulo),
                "Artigo salvo");

            return new JsonNetResult(new { growlMessage });
        }


        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult ExcluiAnexo(int codigoAnexo)
        {
            var anexo = _cadastroArtigos.BuscaAnexo(codigoAnexo);

            if (anexo == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Arquivo não encontrado"));

            var caminhoCompleto= Path.Combine(Server.MapPath("~/Content/Aplicacao/anexos/" + anexo.Artigo.Codigo), anexo.NomeArquivo);
            try
            {
                if (System.IO.File.Exists(caminhoCompleto))
                {
                    System.IO.File.Delete(caminhoCompleto);
                }
                _cadastroArtigos.ExcluiAnexo(anexo);
            }
            catch
            {
                RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Erro ao excluir anexo!"));
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Anexo {0} excluído com sucesso", anexo.NomeArquivo), "Anexo excluído");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult SalvaAnexo(int codigoArtigo, string titulo)
        {
            if (Request.Files == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var artigo = _cadastroArtigos.Busca(codigoArtigo);
            if (artigo == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Artigo não encontrado"));

            if (Request.Files.Count == 0)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var file = Request.Files[0];
            if (file == null)
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Nenhum arquivo anexado"));

            var actualFileName = file.FileName;

            if (artigo.Anexos.Any(a => a.NomeArquivo == actualFileName))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe um anexo com este nome para este artigo!"));

            try
            {
                var path = Server.MapPath("~/Content/Aplicacao/anexos/" + codigoArtigo);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var pathCompleto = Path.Combine(path, actualFileName);
                file.SaveAs(pathCompleto);

                var anexo = new AnexoArtigo
                {
                    Artigo = artigo,
                    NomeArquivo = actualFileName,
                    Titulo = titulo
                };

                _cadastroArtigos.SalvaAnexo(anexo);
            }
            catch (Exception)
            {
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Erro ao anexar arquivo"));
            }

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Anexo {0} salvo com sucesso", actualFileName), "Anexo salvo");

            return new JsonNetResult(new { growlMessage });
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a artigo de id {2}", _user.Name, prefixoOperacao, id);

            var artigo = _cadastroArtigos.Busca(id);
            artigo.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Artigo <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), artigo.Codigo, artigo.Codigo, prefixoOperacao),
                string.Format("Artigo {0}ativado", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Artigo não salvo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoTitulo(Artigo artigo)
        {
            var nomeUpper = artigo.Titulo.ToUpperInvariant();
            var temComMesmoTitulo = _cadastroArtigos
                .BuscaTodos()
                .Any(e => e.Titulo.ToUpperInvariant() == nomeUpper && e.Codigo != artigo.Codigo && artigo.Categoria.Codigo == e.Categoria.Codigo);

            return temComMesmoTitulo;
        }
    }
}