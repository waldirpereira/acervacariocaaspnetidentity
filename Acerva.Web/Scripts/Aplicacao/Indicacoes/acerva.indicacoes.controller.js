(function () {
    "use strict";

    angular.module("acerva.indicacoes")
        .controller("IndicacoesController", ["Indicacoes", IndicacoesController]);

    function IndicacoesController(Indicacoes) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.indicacoesAConfirmar = [];

        ctrl.confirmaIndicacao = confirmaIndicacao;
        ctrl.declinaIndicacao = declinaIndicacao;

        init();

        function init() {
            ctrl.status.carregando = true;
            atualizaListaIndicacoesAConfirmar();
        }

        function atualizaListaIndicacoesAConfirmar() {
            Indicacoes.buscaIndicacoesAConfirmar().then(function (indicacoes) {
                ctrl.indicacoesAConfirmar = indicacoes;
                ctrl.status.carregando = false;
            });
        }

        function confirmaIndicacao(userId) {
            Indicacoes.confirmaIndicacao(userId).then(function () {
                atualizaListaIndicacoesAConfirmar();
            });
        }

        function declinaIndicacao(userId) {
            Indicacoes.declinaIndicacao(userId).then(function () {
                atualizaListaIndicacoesAConfirmar();
            });
        }
    }
})();