using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Acerva.Modelo;
using Acerva.Web.Models;
using Acerva.Web.Models.CadastroPalpites;
using log4net;
using Microsoft.AspNet.Identity;
using NHibernate.Util;

namespace Acerva.Web.Controllers
{
    [Authorize]
    public class PalpiteController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICadastroParticipacoes _cadastroParticipacoes;
        private readonly IPrincipal _user;

        public PalpiteController(ICadastroParticipacoes cadastroParticipacoes, IPrincipal user, ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
            _cadastroParticipacoes = cadastroParticipacoes;
            _user = user;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult BuscaParticipacao([JsonBinder]int codigoParticipacao)
        {
            if (codigoParticipacao == 0)
                return RedirectToAction("Index", "MinhaAcerva");

            var participacao = _cadastroParticipacoes.Busca(codigoParticipacao);

            if (participacao == null)
            {
                return RetornaJsonDeAlerta("Nenhuma participação encontrada.");
            }

            VerificaSeUsuarioPodeBuscarParticipacao(participacao);
            if (!ModelState.IsValid)
            {
                var mensagemValidacao = string.Format("<ul><li>{0}</li></ul>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(me => me.ErrorMessage).Aggregate((a, b) => a + "</li><li>" + b));

                return RetornaJsonDeAlerta(mensagemValidacao);
            }
            
            IncluiPartidasNaoPalpitadas(participacao);

            if (participacao.Usuario.Id != _user.Identity.GetUserId() && !_user.IsInRole("ADMIN"))
                RemovePalpitesFuturos(participacao);

            var participacaoJson = Mapper.Map<ParticipacaoViewModel>(participacao);
            return new JsonNetResult(participacaoJson);
        }

        private void RemovePalpitesFuturos(Participacao participacao)
        {
            participacao.Palpites.Where(p => p.Partida.DataHora > DateTime.Now).ToList().ForEach(
                p =>
                {
                    p.Criterio = null;
                    p.PlacarMandante = null;
                    p.PlacarVisitante = null;
                    p.Pontuacao = null;
                });
        }

        private void IncluiPartidasNaoPalpitadas(Participacao participacao)
        {
            var partidasDoRegional =
                _cadastroParticipacoes.BuscaPartidasDoRegional(participacao.Acerva.Regional.Codigo);

            var partidasNaoPalpitadas = from partida in partidasDoRegional
                                        where participacao.Palpites.All(palpite => palpite.Partida != partida)
                                        select partida;

            var codigo = 0;
            partidasNaoPalpitadas.ForEach(p =>
            {
                var palpite = new Palpite
                {
                    Codigo = --codigo,
                    Participacao = participacao,
                    Partida = p
                };
                participacao.Palpites.Add(palpite);
            });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "Erro!");
            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private void VerificaSeUsuarioPodeBuscarParticipacao(Participacao participacao)
        {
            if (_user.IsInRole("ADMIN"))
                return;

            //usuario logado pertence ao acerva da participacao solicitada
            if (participacao.Acerva.Participacoes.Select(p => p.Usuario.Id).Contains(_user.Identity.GetUserId()))
                return;

            ModelState.AddModelError("CustomError", @"Você não está autorizado a editar ou visualizar estes palpites.");
        }

        [HttpPost]
        [Transacao]
        public ActionResult Salva([JsonBinder]ParticipacaoViewModel participacaoViewModel)
        {
            Log.InfoFormat("Usuário {0} está salvando seus palpites para participação de código {1}", _user.Identity.GetUserId(), participacaoViewModel.Codigo);

            var participacao = _cadastroParticipacoes.Busca(participacaoViewModel.Codigo);

            Mapper.Map(participacaoViewModel, participacao);

            ValidaEdicaoDaParticipacao(participacao);
            if (!ModelState.IsValid)
            {
                var mensagemValidacao = string.Format("Existem erros de validação:<ul><li>{0}</li></ul>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(me => me.ErrorMessage).Aggregate((a, b) => a + "</li><li>" + b));

                return RetornaJsonDeAlerta(mensagemValidacao);
            }

            _cadastroParticipacoes.Salva(participacao);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Palpites da ACervA Carioca <a href='{0}#/{1}'>{2}</a> salvos com sucesso", Url.Action("Index"), participacao.Codigo, participacaoViewModel.NomeAcerva),
                "Palpites salvos");

            return new JsonNetResult(new { growlMessage });
        }

        private void ValidaEdicaoDaParticipacao(Participacao participacao)
        {
            if (participacao.Palpites.Any(p => (p.PlacarMandante == null && p.PlacarVisitante != null) || (p.PlacarMandante != null && p.PlacarVisitante == null)))
                ModelState.AddModelError("CustomError", @"Há pelo menos uma partida com palpite para mandante e não visitante (ou vice versa).");

            if (!_user.IsInRole("ADMIN") && participacao.Usuario.Id != _user.Identity.GetUserId())
                ModelState.AddModelError("CustomError", @"Você não está autorizado a editar esta participação.");
        }
    }
}