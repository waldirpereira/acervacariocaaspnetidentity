(function () {
    "use strict";

    angular.module("acerva.estatuto")
        .controller("EstatutoController", ["$routeParams", "$location", "Estatuto", EstatutoController]);

    function EstatutoController($routeParams, $location, Estatuto) {
        var ctrl = this;

        ctrl.dominio = {};
        ctrl.modelo = null;

        ctrl.status =
        {
            carregando: false
        };

        init();

        function init() {
            buscaEstatuto();
        }

        function buscaEstatuto() {
            ctrl.status.carregando = true;
            Estatuto.busca()
                .then(function (artigo) {
                    ctrl.modelo = artigo;
                })
                .finally(function () { ctrl.status.carregando = false; });
        }
    }
})();