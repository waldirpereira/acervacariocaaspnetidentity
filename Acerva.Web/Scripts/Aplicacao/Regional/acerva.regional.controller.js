(function () {
    "use strict";

    angular.module("acerva.regional")
        .controller("RegionalController", ["Regional", RegionalController]);

    function RegionalController(Regional) {
        var ctrl = this;

        ctrl.dominio = {};

        ctrl.listaRegionais = [];
        
        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            Regional.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                return atualizaListaRegionais();
            });
        }

        function atualizaListaRegionais() {
            Regional.buscaListaRegionais().then(function (listaRegionais) {
                ctrl.listaRegionais = listaRegionais;
            });
        }

         function ativa(regional) {
            Regional.ativa(regional).then(function () {
                regional.ativo = true;
            });
        }

        function inativa(regional) {
            Regional.inativa(regional).then(function () {
                regional.ativo = false;
            });
        }
    }
})();