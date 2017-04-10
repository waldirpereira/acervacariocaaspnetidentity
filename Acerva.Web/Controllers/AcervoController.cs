using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models;
using Acerva.Web.Models.Acervo;

namespace Acerva.Web.Controllers
{
    [AllowAnonymous]
    public class AcervoController : ApplicationBaseController
    {
        private readonly ICadastroArtigos _cadastroArtigos;

        public AcervoController(ICadastroArtigos cadastroArtigos, ICadastroUsuarios cadastroUsuarios) 
            : base(cadastroUsuarios)
        {
            _cadastroArtigos = cadastroArtigos;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaCategorias()
        {
            var usuarioLogado = HttpContext.User;
            
            var listaCategoriasJson = _cadastroArtigos.BuscaCategorias()
                .Where(c => c.Ativo && c.Artigos.Where(a => a.Ativo && a.DataHora <= DateTime.Now).Any(a => a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated)))
                .Select(c =>
                {
                    var categoriaJson = Mapper.Map<CategoriaArtigoViewModel>(c);
                    categoriaJson.QtdArtigos = c.Artigos
                        .Count(a => a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated));
                    return categoriaJson;
                });

            return new JsonNetResult(listaCategoriasJson);
        }

        public ActionResult BuscaArtigosDaCategoria(int codigoCategoriaArtigo)
        {
            var usuarioLogado = HttpContext.User;

            var listaArtigosJson = _cadastroArtigos.BuscaTodos()
                .Where(a => a.Categoria.Codigo == codigoCategoriaArtigo &&
                            a.Ativo && 
                            a.DataHora <= DateTime.Now &&
                            (a.Visibilidade == VisibilidadeArtigo.Publico || (a.Visibilidade == VisibilidadeArtigo.Autenticado && usuarioLogado.Identity.IsAuthenticated)))
                 .OrderBy(a => a.DataHora)
                 .ThenBy(a => a.Codigo)
                .Select(Mapper.Map<ArtigoListaViewModel>);
            return new JsonNetResult(listaArtigosJson);
        }
        
        public ActionResult Busca(int codigo)
        {
            var artigo = _cadastroArtigos.Busca(codigo);
            if (!artigo.Ativo)
            {
                return RetornaJsonDeAlerta("O artigo buscado não está ativo.");
            }

            if (artigo.DataHora > DateTime.Now)
            {
                return RetornaJsonDeAlerta(string.Format("O artigo buscado não liberado para visualização (data de publicação: {0}).", artigo.DataHora.ToString("dd/MM/yyyy HH:mm")));
            }

            var artigoJson = Mapper.Map<ArtigoViewModel>(artigo);
            return new JsonNetResult(artigoJson);
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Erro ao buscar artigo");

            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }
    }
}