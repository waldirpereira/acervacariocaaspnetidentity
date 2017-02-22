(function () {
    "use strict";

    angular.module("acerva.carteirinha")
        .controller("CarteirinhaController", ["Carteirinha", CarteirinhaController]);

    function CarteirinhaController(Carteirinha) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.dominio = {};

        ctrl.cpf = null;
        ctrl.modelo = null;

        ctrl.busca = busca;
        
        init();

        function init() {
            return busca();
        }

        function busca() {
            ctrl.status.carregando = true;
            return Carteirinha.busca().then(function (usuario) {
                ctrl.modelo = usuario || {};
                ctrl.status.carregando = false;
            });
        }
    }
})();