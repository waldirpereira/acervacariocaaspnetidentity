using System.Collections.Generic;
using System.Dynamic;
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
using Acerva.Web.Models.CadastroAcervas;
using FluentValidation;
using log4net;
using Microsoft.AspNet.Identity;

namespace Acerva.Web.Controllers
{
    [Authorize]
    public class MinhaAcervaController : ApplicationBaseController
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICadastroAcervas _cadastroAcervas;
        private readonly IValidator<Modelo.Acerva> _validator;
        private readonly ICadastroParticipacoes _cadastroParticipacoes;
        private readonly IIdentity _user;
        private readonly EmailService _mailService;

        public MinhaAcervaController(ICadastroAcervas cadastroAcervas, IValidator<Modelo.Acerva> validator, IPrincipal user, ICadastroUsuarios cadastroUsuarios,
            ICadastroParticipacoes cadastroParticipacoes) : base(cadastroUsuarios)
        {
            _cadastroAcervas = cadastroAcervas;
            _validator = validator;
            _cadastroParticipacoes = cadastroParticipacoes;
            _mailService = new EmailService();
            _user = user.Identity;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult BuscaMinhasAcervasParaListagem()
        {
            var acervas = User.IsInRole("ADMIN")
                ? _cadastroAcervas.BuscaParaListagem()
                : _cadastroAcervas.BuscaTodosEmQueUsuarioParticipa(_user.GetUserId()).Where(b => b.Ativo);

            var minhasAcervas = acervas.Select(Mapper.Map<AcervaViewModel>);

            return new JsonNetResult(minhasAcervas);
        }

        public ActionResult BuscaTiposDominio()
        {
            var regionaisJson = _cadastroAcervas.BuscaRegionais()
                .Where(c => c.Ativo)
                .Select(Mapper.Map<RegionalViewModel>);

            var novasRegrasJson = RecuperaListaNovasRegrasComPontuacoesPadrao(new Modelo.Acerva())
                .Select(Mapper.Map<RegraViewModel>);

            return new JsonNetResult(new
            {
                Regionais = regionaisJson,
                NovasRegras = novasRegrasJson
            });
        }

        public ActionResult Busca(int codigo)
        {
            var acerva = _cadastroAcervas.Busca(codigo);

            ValidaSeUsuarioPodeVisualiazarAcerva(acerva);
            var validacao = _validator.Validate(acerva);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            var rodada = BuscaUltimaRodadaComPartidaTerminada(acerva);
            acerva.CalculaPontuacaoEPosicaoDasParticipacoes(rodada);
            IncluiRegrasParaCriteriosSemPontuacao(acerva);

            var acervaJson = Mapper.Map<AcervaViewModel>(acerva);
            return new JsonNetResult(acervaJson);
        }

        public ActionResult BuscaEvolucao(int codigo)
        {
            var acerva = _cadastroAcervas.Busca(codigo);

            ValidaSeUsuarioPodeVisualiazarAcerva(acerva);
            if (!ModelState.IsValid)
            {
                var mensagemValidacao = string.Format("Existem erros de validação:<ul><li>{0}</li></ul>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(me => me.ErrorMessage).Aggregate((a, b) => a + "</li><li>" + b));

                return RetornaJsonDeAlerta(mensagemValidacao);
            }

            var palpitesComPontuacao = (from p in acerva.Participacoes
                from palpite in p.Palpites
                where palpite.Criterio != null
                select palpite).ToList();

            var menorRodadaEntrePalpitesComPontuacao = palpitesComPontuacao.Select(p => p.Partida.Rodada).OrderBy(r => r.Ordem).FirstOrDefault();
            if (menorRodadaEntrePalpitesComPontuacao == null)
                return null;

            var maiorRodadaEntrePalpitesComPontuacao = palpitesComPontuacao.Select(p => p.Partida.Rodada).OrderByDescending(r => r.Ordem).FirstOrDefault();
            if (maiorRodadaEntrePalpitesComPontuacao == null)
                return null;

            var rodadas = new List<string> {"0"};
            
            var registrosEvolucao = acerva.Participacoes.Select(participacao => new RegistroEvolucao(participacao)).ToList();

            foreach (var rodada in acerva.Regional.Rodadas.OrderBy(r => r.Ordem))
            {
                rodadas.Add(rodada.Nome);

                acerva.CalculaPontuacaoEPosicaoDasParticipacoes(rodada);
                foreach (var participacao in acerva.Participacoes)
                {
                    var participacaoNoRetorno = registrosEvolucao.FirstOrDefault(p => p.Codigo == participacao.Codigo);
                    participacaoNoRetorno?.Posicoes.Add(rodada.Ordem < menorRodadaEntrePalpitesComPontuacao.Ordem || rodada.Ordem > maiorRodadaEntrePalpitesComPontuacao.Ordem
                        ? (int?) null 
                        : participacao.Posicao);
                    participacaoNoRetorno?.Pontuacoes.Add(rodada.Ordem < menorRodadaEntrePalpitesComPontuacao.Ordem || rodada.Ordem > maiorRodadaEntrePalpitesComPontuacao.Ordem
                        ? null
                        : participacao.PontuacaoAtual);
                }
            }
            return new JsonNetResult(new
            {
                Rodadas = rodadas,
                RegistrosEvolucao = registrosEvolucao
            });
        }

        private static Rodada BuscaUltimaRodadaComPartidaTerminada(Modelo.Acerva acerva)
        {
            var rodadasComAlgumaPartidaTerminada =
                acerva.Regional.Rodadas.Where(r => r.Partidas.Any(p => p.Terminada)).ToList();
            var rodada = rodadasComAlgumaPartidaTerminada
                .OrderByDescending(r => r.Ordem)
                .FirstOrDefault();
            return rodada;
        }

        private void IncluiRegrasParaCriteriosSemPontuacao(Modelo.Acerva acerva)
        {
            acerva.Regras.ToList().AddRange(RecuperaListaNovasRegrasComPontuacoesPadrao(acerva));
        }

        private IEnumerable<Regra> RecuperaListaNovasRegrasComPontuacoesPadrao(Modelo.Acerva acerva)
        {
            var criterios = _cadastroAcervas.BuscaCriterios();
            var criteriosSemRegras = criterios.Where(c => !acerva.Regras.Select(r => r.Criterio).Contains(c));
            var codigo = 0;

            var regras = new List<Regra>();
            criteriosSemRegras.ToList().ForEach(c =>
            {
                regras.Add(new Regra { Codigo = --codigo, Acerva = acerva, Criterio = c, Pontuacao = c.PontuacaoPadrao });
            });

            return regras;
        }

        [Transacao]
        [HttpPost]
        public ActionResult SalvaMinhaAcerva([JsonBinder]AcervaViewModel acervaViewModel)
        {
            return Salva(acervaViewModel, true);
        }

        [Transacao]
        [HttpPost]
        public ActionResult Salva([JsonBinder]AcervaViewModel acervaViewModel, bool veioDoMinhaAcerva = false)
        {
            Log.InfoFormat("Usuário está salvando a ACervA Carioca {0} (regional {1}) de código {2}", acervaViewModel.Nome, acervaViewModel.Regional.Nome, acervaViewModel.Codigo);

            var ehNovo = acervaViewModel.Codigo == 0;
            var acerva = ehNovo ? new Modelo.Acerva() : _cadastroAcervas.Busca(acervaViewModel.Codigo);

            acervaViewModel.Nome = acervaViewModel.Nome.Trim();

            var mensagensNovosParticipantes = new List<IdentityMessage>();
            acervaViewModel.Participacoes.Where(p => p.Codigo <= 0 && p.Usuario.Id != acervaViewModel.UsuarioResponsavel.Id).ToList()
                .ForEach(p => { mensagensNovosParticipantes.Add(CriaMensagemEmailConviteParaNovoParticipante(p, acervaViewModel)); });

            Mapper.Map(acervaViewModel, acerva);

            ValidaSeUsuarioPodeEditarAcerva(acerva);
            if (!ModelState.IsValid)
            {
                var mensagemValidacao = string.Format("Existem erros de validação:<ul><li>{0}</li></ul>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(me => me.ErrorMessage).Aggregate((a, b) => a + "</li><li>" + b));

                return RetornaJsonDeAlerta(mensagemValidacao);
            }

            var validacao = _validator.Validate(acerva);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            _cadastroAcervas.BuscaUsuariosJaCadastrados(acerva);

            if (ExisteComMesmoNome(acerva))
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "Já existe uma ACervA com o nome {0:unsafe}", acerva.Nome));

            _cadastroAcervas.Salva(acerva);

            mensagensNovosParticipantes.ForEach(m => { _mailService.SendAsync(m); });

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("ACervA Carioca <a href='{0}#/Edit/{1}'>{2}</a> foi salvo com sucesso", veioDoMinhaAcerva ? Url.Action("Index", "MinhaAcerva") : Url.Action("Index"), acerva.Codigo, acerva.Nome),
                "ACervA Carioca salvo");

            return new JsonNetResult(new { growlMessage });
        }

        private IdentityMessage CriaMensagemEmailConviteParaNovoParticipante(ParticipacaoViewModel participacaoViewModel, AcervaViewModel acervaViewModel)
        {
            var usuarioBd = CadastroUsuarios.BuscaPeloEmail(participacaoViewModel.Usuario.Email);
            var jaPossuiCadastro = usuarioBd != null;
            var mensagemLogin = !jaPossuiCadastro
                ? "Como você ainda não possui cadastro, acesse nosso sistema e realize o cadastro utilizando seu e-mail."
                : "Como você já possui cadastro em nosso site, basta acessar sua conta e participar da ACervA Carioca já criado.";

            var url = jaPossuiCadastro
                ? Url.AbsoluteAction("Index", "Home")
                : Url.AbsoluteAction("Register", "Account");

            var mensagem = string.Format("Olá {0},<br /><br />" +
                                         "Você foi convidado por {1} a participar da ACervA Carioca {2}.<br /><br />" +
                                         "{3}<br /><br />" +
                                         "{4}<br /><br />" +
                                         "Um abraço,<br />" +
                                         "Equipe {5}", jaPossuiCadastro ? usuarioBd.Name : participacaoViewModel.Usuario.Name,
                acervaViewModel.UsuarioResponsavel.Name, acervaViewModel.Nome, mensagemLogin, url, Sistema.NomeSistema);

            return new IdentityMessage { Destination = participacaoViewModel.Usuario.Email, Subject = "Você recebeu um convite para uma ACervA!", Body = mensagem };
        }

        private void ValidaSeUsuarioPodeEditarAcerva(Modelo.Acerva acerva)
        {
            if (acerva.Codigo == 0)
                return;

            if (acerva.UsuarioResponsavel.Id != _user.GetUserId() && !User.IsInRole("ADMIN"))
                ModelState.AddModelError("Nome", @"Você não tem autorização para alterar esta ACervA.");
        }

        private void ValidaSeUsuarioPodeVisualiazarAcerva(Modelo.Acerva acerva)
        {
            var userId = _user.GetUserId();
            if (acerva.UsuarioResponsavel.Id != userId && !User.IsInRole("ADMIN") && acerva.Participacoes.All(p => p.Usuario.Id != userId))
                ModelState.AddModelError("Nome", @"Você não tem autorização para visualizar esta ACervA.");
        }

        [Transacao]
        [HttpPost]
        [ValidateAjaxAntiForgeryToken]
        [AcervaAuthorize(Roles = "ADMIN")]
        public ActionResult AlteraAtivacao(int id, bool ativo)
        {
            var prefixoOperacao = ativo ? string.Empty : "des";
            Log.InfoFormat("Usuário {0} está {1}atividando a ACervA Carioca de id {2}", _user.Name, prefixoOperacao, id);

            var acerva = _cadastroAcervas.Busca(id);

            ValidaSeUsuarioPodeEditarAcerva(acerva);
            var validacao = _validator.Validate(acerva);
            if (!validacao.IsValid)
                return RetornaJsonDeAlerta(validacao.GeraListaHtmlDeValidacoes());

            acerva.Ativo = ativo;

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("ACervA Carioca <a href='{0}#/Edit/{1}'>{2}</a> foi {3}ativado com sucesso", Url.Action("Index"), acerva.Codigo, acerva.Codigo, prefixoOperacao),
                string.Format("ACervA Carioca {0}ativado", prefixoOperacao));

            return new JsonNetResult(new { growlMessage });
        }

        private static ActionResult RetornaJsonDeAlerta(string mensagem)
        {
            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Warning, mensagem, "ACervA Carioca não salvo");
            return new JsonNetResult(new { growlMessage }, statusCode: JsonNetResult.HttpBadRequest);
        }

        private bool ExisteComMesmoNome(Modelo.Acerva acerva)
        {
            var nomeUpper = acerva.Nome.ToUpperInvariant();
            var temComMesmoNome = _cadastroAcervas
                .BuscaTodos()
                .Any(e => e.Nome.ToUpperInvariant() == nomeUpper && e.Codigo != acerva.Codigo);

            return temComMesmoNome;
        }

        [HttpPost]
        public ActionResult ReenviaConviteParticipacao([JsonBinder]ParticipacaoViewModel participacaoViewModel)
        {
            if (participacaoViewModel.Codigo <= 0)
            {
                return RetornaJsonDeAlerta(string.Format(HtmlEncodeFormatProvider.Instance, "A participação ainda não foi salva."));
            }
            var participacao = _cadastroParticipacoes.Busca(participacaoViewModel.Codigo);

            var acerva = _cadastroAcervas.Busca(participacaoViewModel.CodigoAcerva);
            ValidaSeUsuarioPodeEditarAcerva(acerva);
            if (!ModelState.IsValid)
            {
                var mensagemValidacao = string.Format("Existem erros de validação:<ul><li>{0}</li></ul>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(me => me.ErrorMessage).Aggregate((a, b) => a + "</li><li>" + b));

                return RetornaJsonDeAlerta(mensagemValidacao);
            }


            Log.InfoFormat("Usuário {0} está reenviando mensagem de convite para a ACervA Carioca de id {1} para o usuário de e-mail {2}",
                _user.Name, participacao.Acerva.Codigo, participacao.Usuario.Email);

            var mensagem = CriaMensagemEmailConviteParaNovoParticipante(participacaoViewModel,
                new AcervaViewModel
                {
                    UsuarioResponsavel = new IdentityUserViewModel { Name = participacao.Acerva.UsuarioResponsavel.Name },
                    Nome = participacao.Acerva.Nome
                });

            _mailService.SendAsync(mensagem);

            var growlMessage = new GrowlMessage(GrowlMessageSeverity.Success,
                string.Format("Convite reenviado com sucesso para {0}", participacao.Usuario.Email), "Convite reenviado");

            return new JsonNetResult(new { growlMessage });
        }
    }

    class RegistroEvolucao
    {
        public RegistroEvolucao(Participacao participacao)
        {
            Codigo = participacao.Codigo;
            Nome = participacao.Usuario.Name;
            Posicoes.Add(participacao.PosicaoInicial);
            Pontuacoes.Add(participacao.PontuacaoInicial);
        }

        public int Codigo { get; set; }
        public string Nome { get; set; }

        private List<int?> _posicoes = new List<int?>();
        public List<int?> Posicoes
        {
            get { return _posicoes; }
            set { _posicoes = value; }
        }

        private List<int?> _pontuacoes = new List<int?>();
        public List<int?> Pontuacoes
        {
            get { return _pontuacoes; }
            set { _pontuacoes = value; }
        }
    }
}