(function () {
    "use strict";

    angular.module("acerva.palpite")
        .controller("PalpiteController", ["$scope", "$timeout", "$routeParams", "$window", "$location", "ROTAS", "Palpite", PalpiteController]);

    function PalpiteController($scope, $timeout, $routeParams, $window, $location, ROTAS, Palpite) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.listaPalpites = [];

        ctrl.placarMandantePadrao = null;
        ctrl.placarVisitantePadrao = null;

        ctrl.salvaPalpites = salvaPalpites;
        ctrl.podeEditarPartida = podeEditarPartida;
        ctrl.buscaRodadas = buscaRodadas;
        ctrl.alteraPalpitesDaRodada = alteraPalpitesDaRodada;
        ctrl.alteraPalpitesDaRodadaAnterior = alteraPalpitesDaRodadaAnterior;
        ctrl.buscaRodadaAnterior = buscaRodadaAnterior;
        ctrl.alteraPalpitesDaRodadaPosterior = alteraPalpitesDaRodadaPosterior;
        ctrl.buscaRodadaPosterior = buscaRodadaPosterior;
        ctrl.podeEditarAlgumaPartida = podeEditarAlgumaPartida;
        ctrl.replicaPlacarPadrao = replicaPlacarPadrao;

        ctrl.incrementa = incrementa;
        ctrl.decrementa = decrementa;

        var idParticipacao = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idParticipacao);
        
        function init(id) {
            ctrl.status.carregando = true;

            if (!id) {
                $window.location.href = ROTAS.minhasAcervas;
            }

            return Palpite.buscaParticipacao(id).then(function (participacao) {
                colocaParticipacaoEmEdicao(participacao);

                ctrl.listaRodadas = buscaRodadas();
                ctrl.status.carregando = false;
            });
        }

        function colocaParticipacaoEmEdicao(participacao) {
            ctrl.status.bloqueado = !participacao.acervaAtivo;
            ctrl.modeloOriginal = participacao;
            ctrl.modelo = angular.copy(participacao);

            var proximaPartida = _(participacao.palpites).map('partida').orderBy(function (p) { return retornaDataJsonUtcPartida(p); }).find(function (p) { return partidaFutura(p); });
            if (proximaPartida)
                alteraPalpitesDaRodada({ codigo: proximaPartida.codigoRodada, nome: proximaPartida.nomeRodada, ordem: proximaPartida.ordemRodada });
        }

        function buscaRodadas() {
            if (!ctrl.modelo.palpites)
                return null;

            return _(ctrl.modelo.palpites)
                .map(function (p) { return { codigo: p.partida.codigoRodada, nome: p.partida.nomeRodada, ordem: p.partida.ordemRodada }; })
                .uniqBy('codigo')
                .value();
        }

        function alteraPalpitesDaRodada(rodada) {
            ctrl.rodadaEmEdicao = rodada;
            ctrl.palpitesDaRodadaEmEdicao = _.filter(ctrl.modelo.palpites, function(p) { return p.partida.codigoRodada === rodada.codigo; });
        }

        function alteraPalpitesDaRodadaAnterior() {
            var proximaRodada = buscaRodadaAnterior();
            if (!!proximaRodada)
                alteraPalpitesDaRodada(proximaRodada);
        }

        function buscaRodadaAnterior() {
            return _(ctrl.listaRodadas).orderBy('ordem').findLast(function (r) { return r.ordem < ctrl.rodadaEmEdicao.ordem; });
        }

        function alteraPalpitesDaRodadaPosterior() {
            var proximaRodada = buscaRodadaPosterior();
            if (!!proximaRodada)
                alteraPalpitesDaRodada(proximaRodada);
        }

        function buscaRodadaPosterior() {
            return _(ctrl.listaRodadas).orderBy('ordem').find(function (r) { return r.ordem > ctrl.rodadaEmEdicao.ordem; });
        }

        function podeEditarPartida(partida) {
            if (partida.terminada)
                return false;

            if (ctrl.modelo.usuario.id !== AcervaApp.user.id)
                return false;

            return partidaFutura(partida);
        }
        function podeEditarAlgumaPartida() {
            return !!_(ctrl.palpitesDaRodadaEmEdicao).find(function(p) { return podeEditarPartida(p.partida); });
        }
        function replicaPlacarPadrao() {
            _(ctrl.palpitesDaRodadaEmEdicao).forEach(function (palpite) {
                if (!podeEditarPartida(palpite.partida))
                    return;
                palpite.placarMandante = ctrl.placarMandantePadrao;
                palpite.placarVisitante = ctrl.placarVisitantePadrao;
            });
        }

        function partidaFutura(partida) {
            var dataHoje = moment.now();
            return retornaDataJsonUtcPartida(partida) > moment(dataHoje).format("YYYY-MM-DDTHH:mm:ss");
        }

        function retornaDataJsonUtcPartida(partida) {
            var dataJson = partida.data;
            var dataJsonUtc = moment(dataJson).utc();
            dataJsonUtc.hours(partida.horario.hour);
            dataJsonUtc.minutes(partida.horario.minute);
            return dataJsonUtc.format("YYYY-MM-DDTHH:mm:ss");
        }
        
        function salvaPalpites() {
            if ($scope.formParticipacao.$invalid)
                return;

            ctrl.status.salvando = true;

            Palpite.salva(ctrl.modelo)
                .then(function () { $location.path("/" + ctrl.modelo.codigo); })
                .finally(function () { ctrl.status.salvando = false; });
        }

        function incrementa(valor) {
            valor = valor !== null ? +valor : null;
            if (!_.isFinite(valor)) {
                return 0;
            }
            return ++valor;
        }

        function decrementa(valor) {
            valor = +valor;
            if (!_.isFinite(valor) || valor === 0) {
                return 0;
            }
            return --valor;
        }
    }
})();