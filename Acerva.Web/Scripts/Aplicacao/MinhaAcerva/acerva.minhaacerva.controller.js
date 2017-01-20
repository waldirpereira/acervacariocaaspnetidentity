(function () {
    "use strict";

    angular.module("acerva.minhaAcerva")
        .controller("MinhaAcervaController", ["MinhaAcerva", MinhaAcervaController]);

    function MinhaAcervaController(MinhaAcerva) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.listaMinhasAcervas = [];
        
        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            ctrl.status.carregando = true;
            atualizaListaMinhasAcervas();
        }

        function atualizaListaMinhasAcervas() {
            MinhaAcerva.buscaListaMinhasAcervas().then(function (listaMinhasAcervas) {
                ctrl.listaMinhasAcervas = listaMinhasAcervas;
                ctrl.status.carregando = false;
            });
        }

        function ativa(acerva) {
            MinhaAcerva.ativa(acerva).then(function () {
                acerva.ativo = true;
            });
        }

        function inativa(acerva) {
            MinhaAcerva.inativa(acerva).then(function () {
                acerva.ativo = false;
            });
        }
    }
})();