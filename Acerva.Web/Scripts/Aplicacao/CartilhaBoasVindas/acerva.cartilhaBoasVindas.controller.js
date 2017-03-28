(function () {
    "use strict";

    angular.module("acerva.cartilhaBoasVindas")
        .controller("CartilhaBoasVindasController", ["$routeParams", "$location", "CartilhaBoasVindas", CartilhaBoasVindasController]);

    function CartilhaBoasVindasController($routeParams, $location, CartilhaBoasVindas) {
        var ctrl = this;

        ctrl.dominio = {};
        ctrl.modelo = null;

        ctrl.status =
        {
            carregando: false
        };

        init();

        function init() {
            buscaCartilhaBoasVindas();
        }

        function buscaCartilhaBoasVindas() {
            ctrl.status.carregando = true;
            CartilhaBoasVindas.busca()
                .then(function (artigo) {
                    ctrl.modelo = artigo;
                })
                .finally(function () { ctrl.status.carregando = false; });
        }
    }
})();