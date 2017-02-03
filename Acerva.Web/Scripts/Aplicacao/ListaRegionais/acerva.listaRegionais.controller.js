(function () {
    "use strict";

    angular.module("acerva.listaRegionais")
        .controller("ListaRegionaisController", ["ListaRegionais", ListaRegionaisController]);

    function ListaRegionaisController(ListaRegionais) {
        var ctrl = this;

        ctrl.listaRegionais = [];

        init();

        function init() {
            atualizaListaRegionais();
        }

        function atualizaListaRegionais() {
            ListaRegionais.buscaRegionais().then(function (listaRegionais) {
                ctrl.listaRegionais = listaRegionais;
            });
        }
    }
})();