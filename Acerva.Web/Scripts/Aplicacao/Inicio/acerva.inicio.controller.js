(function () {
    "use strict";

    angular.module("acerva.inicio")
        .controller("InicioController", ["Inicio", InicioController]);

    function InicioController(Inicio) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.listaMinhasAcervas = [];

        init();

        function init() {
            ctrl.status.carregando = true;
            atualizaListaMinhasAcervas();
        }

        function atualizaListaMinhasAcervas() {
            Inicio.buscaListaMinhasAcervas().then(function (minhasAcervas) {
                ctrl.listaMinhasAcervas = minhasAcervas;
                ctrl.status.carregando = false;
            });
        }
    }
})();