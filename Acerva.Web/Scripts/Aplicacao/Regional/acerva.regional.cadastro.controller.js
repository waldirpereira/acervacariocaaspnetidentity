(function () {
    "use strict";

    angular.module("acerva.regional")
        .controller("CadastroRegionalController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Regional", CadastroRegionalController]);

    function CadastroRegionalController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Regional) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.equipeEmEdicao = null;
        ctrl.equipeEmEdicaoOriginal = null;
        
        ctrl.rodadaEmEdicao = null;
        ctrl.rodadaEmEdicaoOriginal = null;
        ctrl.codigoRodadaInicial = null;

        ctrl.partidaEmEdicao = null;
        ctrl.partidaEmEdicaoOriginal = null;
        ctrl.dataEmEdicao = null;

        ctrl.salvaRegional = salvaRegional;

        ctrl.incluiRodada = incluiRodada;
        ctrl.salvaRodada = salvaRodada;
        ctrl.editaRodada = editaRodada;
        ctrl.excluiRodada = excluiRodada;
        ctrl.cancelaEdicaoRodada = cancelaEdicaoRodada;
        ctrl.rodadaAberta = rodadaAberta;
        ctrl.primeiraRodadaAberta = primeiraRodadaAberta;
        
        ctrl.incluiPartida = incluiPartida;
        ctrl.salvaPartida = salvaPartida;
        ctrl.editaPartida = editaPartida;
        ctrl.excluiPartida = excluiPartida;
        ctrl.cancelaEdicaoPartida = cancelaEdicaoPartida;
        ctrl.filtraEquipes = filtraEquipes;
        ctrl.marcaPartidasComoTerminadas = marcaPartidasComoTerminadas;
        ctrl.carregarPlacares = carregarPlacares;

        ctrl.incrementa = incrementa;
        ctrl.decrementa = decrementa;

        var idRegional = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idRegional);

        function init(id) {
            ctrl.status.carregando = true;

            Regional.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaRegionalEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Regional.buscaRegional(id).then(function(regional) {
                    colocaRegionalEmEdicao(regional);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaRegionalEmEdicao(regional) {
            ctrl.status.bloqueado = regional.codigo && !regional.ativo;
            ctrl.modeloOriginal = regional;
            ctrl.modelo = angular.copy(regional);

            var proximaRodada = _(ctrl.modelo.rodadas).find(function (r) { return ctrl.rodadaAberta(r); });
            if (proximaRodada) {
                editaRodada(proximaRodada);
                ctrl.codigoRodadaInicial = proximaRodada.codigo;
            }
        }

        function salvaRegional() {
            if ($scope.formRegional.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Regional.salvaRegional(ctrl.modelo)
                .then(function () {  })
                .finally(function () { ctrl.status.salvando = false; });
        }

        // RODADA - INICIO
        function incluiRodada() {
            if (!ctrl.modelo.rodadas)
                ctrl.modelo.rodadas = [];

            var minCodigo = (_.minBy(ctrl.modelo.rodadas, "codigo") || {}).codigo;
            minCodigo = minCodigo < 0 ? minCodigo : 0;
            var nova = { codigo: minCodigo - 1, ordem: ctrl.modelo.rodadas.length + 1, partidas: [] };

            ctrl.rodadaEmEdicao = nova;
            ctrl.modelo.rodadas.push(nova);
        }

        function editaRodada(rodada) {
            if (ctrl.rodadaEmEdicao)
                cancelaEdicaoRodada();
            ctrl.rodadaEmEdicaoOriginal = angular.copy(rodada);
            ctrl.rodadaEmEdicao = rodada;
        }

        function excluiRodada(rodada) {
            var indiceRodadaExcluida = _.findIndex(ctrl.modelo.rodadas, function (f) {
                return f.codigo === rodada.codigo;
            });

            _.pullAt(ctrl.modelo.rodadas, indiceRodadaExcluida);
        }

        function cancelaEdicaoRodada() {
            if (!ctrl.rodadaEmEdicaoOriginal) {
                _.remove(ctrl.modelo.rodadas, function (f) {
                    return f.codigo === ctrl.rodadaEmEdicao.codigo;
                });
            } else {
                var indiceRodadaEditada = _.findIndex(ctrl.modelo.rodadas, function (f) {
                    return f.codigo === ctrl.rodadaEmEdicaoOriginal.codigo;
                });
                ctrl.modelo.rodadas[indiceRodadaEditada] = angular.copy(ctrl.rodadaEmEdicaoOriginal);
            }

            ctrl.rodadaEmEdicao = null;
            ctrl.rodadaEmEdicaoOriginal = null;
        }

        function salvaRodada() {
            ctrl.rodadaEmEdicao = null;
            ctrl.rodadaEmEdicaoOriginal = null;
        }

        function rodadaAberta(rodada) {
            return !!_(rodada.partidas).find(function(p) { return !p.terminada });
        }
        function primeiraRodadaAberta(rodada) {
            return _(ctrl.modelo.rodadas).find(function (r) { return ctrl.rodadaAberta(r); }).codigo === rodada.codigo;
        }
        // RODADA - FIM


        // PARTIDA - INICIO
        function incrementa(valor) {
            valor = valor  !== null ? +valor : null;
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

        function incluiPartida() {
            if (!ctrl.rodadaEmEdicao.partidas)
                ctrl.rodadaEmEdicao.partidas = [];

            var minCodigo = (_.minBy(ctrl.rodadaEmEdicao.partidas, "codigo") || {}).codigo;
            minCodigo = minCodigo < 0 ? minCodigo : 0;
            var nova = { codigo: minCodigo - 1, ordem: ctrl.rodadaEmEdicao.partidas.length + 1, partidas: [] };

            ctrl.partidaEmEdicao = nova;
            ctrl.rodadaEmEdicao.partidas.push(nova);
        }

        function editaPartida(partida) {
            if (ctrl.partidaEmEdicao)
                cancelaEdicaoPartida();

            ctrl.partidaEmEdicaoOriginal = angular.copy(partida);
            ctrl.partidaEmEdicao = partida;
            ctrl.dataEmEdicao = partida.data;
        }

        function excluiPartida(partida) {
            var indicePartidaExcluida = _.findIndex(ctrl.rodadaEmEdicao.partidas, function (f) {
                return f.codigo === partida.codigo;
            });

            _.pullAt(ctrl.rodadaEmEdicao.partidas, indicePartidaExcluida);
        }

        function cancelaEdicaoPartida() {
            if (!ctrl.partidaEmEdicaoOriginal) {
                _.remove(ctrl.rodadaEmEdicao.partidas, function (f) {
                    return f.codigo === ctrl.partidaEmEdicao.codigo;
                });
            } else {
                var indicePartidaEditada = _.findIndex(ctrl.rodadaEmEdicao.partidas, function (f) {
                    return f.codigo === ctrl.partidaEmEdicaoOriginal.codigo;
                });
                ctrl.rodadaEmEdicao.partidas[indicePartidaEditada] = angular.copy(ctrl.partidaEmEdicaoOriginal);
            }

            ctrl.partidaEmEdicao = null;
            ctrl.partidaEmEdicaoOriginal = null;
            ctrl.dataEmEdicao = null;
        }

        function salvaPartida() {
            ctrl.partidaEmEdicao.data = ctrl.dataEmEdicao;
            if (!validaPartida(ctrl.partidaEmEdicao))
                return;

            ctrl.partidaEmEdicao = null;
            ctrl.partidaEmEdicaoOriginal = null;
            ctrl.dataEmEdicao = null;
        }

        function validaPartida(partida) {
            if (!partida.equipeMandante || !partida.equipeVisitante) {
                CanalMensagemGrowl.enviaNovaMensagem({
                    message: "As duas equipes devem ser informadas.",
                    severity: "error"
                });
                return false;
            }


            if (partida.equipeMandante.codigo === partida.equipeVisitante.codigo) {
                CanalMensagemGrowl.enviaNovaMensagem({
                    message: "As equipes devem ser diferentes.",
                    severity: "error"
                });
                return false;
            }
            
            if (!partida.data) {
                CanalMensagemGrowl.enviaNovaMensagem({
                    message: "A data deve ser informada.",
                    severity: "error"
                });
                return false;
            }
            
            if (!partida.horario || !partida.horario.hour || !partida.horario.minute) {
                CanalMensagemGrowl.enviaNovaMensagem({
                    message: "O horário deve ser informado.",
                    severity: "error"
                });
                return false;
            }

            return true;
        }

        function filtraEquipes() {
            return _(ctrl.modelo.equipes).filter(function(equipe) {
                return !_(ctrl.rodadaEmEdicao.partidas).find(function(partida) {
                    return partida.codigo !== ctrl.partidaEmEdicao.codigo && (partida.equipeMandante.codigo === equipe.codigo || partida.equipeVisitante.codigo === equipe.codigo);
                });
            }).value();
        }

        function marcaPartidasComoTerminadas() {
            _(ctrl.rodadaEmEdicao.partidas).forEach(function (partida) {
                if (partida.placarMandante !== null && partida.placarVisitante !== null)
                    partida.terminada = true;
            });
        }

        function carregarPlacares() {
            Regional.carregarPlacares(ctrl.rodadaEmEdicao.codigo).then(function (partidas) {
                var placarImportado = 0;
                _(partidas).forEach(function(partida) {
                    var partidaNoModelo = _(ctrl.rodadaEmEdicao.partidas).find(function (p) {
                        return p.codigo === partida.codigo && !p.terminada;
                    });
                    if (!partidaNoModelo)
                        return;

                    partidaNoModelo.placarMandante = partida.placarMandante;
                    partidaNoModelo.placarVisitante = partida.placarVisitante;
                    partidaNoModelo.data = partida.data;
                    partidaNoModelo.horario = partida.horario;
                    placarImportado++;
                });

                CanalMensagemGrowl.enviaNovaMensagem({
                    message: placarImportado + " placar(es) importado(s).",
                    severity: "success"
                });
            });
        }
        // PARTIDA - FIM
    }

})();