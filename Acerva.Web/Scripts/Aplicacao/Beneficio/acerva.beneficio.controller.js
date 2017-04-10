(function () {
    "use strict";

    angular.module("acerva.beneficio")
        .controller("BeneficioController", ["Beneficio", BeneficioController]);

    function BeneficioController(Beneficio) {
        var ctrl = this;

        ctrl.listaBeneficios = [];

        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaBeneficios();
        }

        function atualizaListaBeneficios() {
            Beneficio.buscaListaBeneficios().then(function (listaBeneficios) {
                ctrl.listaBeneficios = listaBeneficios;
            });
        }

        function ativa(beneficio) {
            Beneficio.ativa(beneficio).then(function () {
                beneficio.ativo = true;
            });
        }

        function inativa(beneficio) {
            Beneficio.inativa(beneficio).then(function () {
                beneficio.ativo = false;
            });
        }
    }
})();