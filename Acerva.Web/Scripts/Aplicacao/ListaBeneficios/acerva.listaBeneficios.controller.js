(function () {
    "use strict";

    angular.module("acerva.listaBeneficios")
        .controller("ListaBeneficiosController", ["ListaBeneficios", ListaBeneficiosController]);

    function ListaBeneficiosController(ListaBeneficios) {
        var ctrl = this;

        ctrl.listaBeneficios = [];

        init();

        function init() {
            atualizaListaBeneficios();
        }

        function atualizaListaBeneficios() {
            ListaBeneficios.buscaBeneficios().then(function (listaBeneficios) {
                ctrl.listaBeneficios = listaBeneficios;
            });
        }
    }
})();