(function () {
    "use strict";

    angular.module("acerva.equipe")
        .controller("EquipeController", ["Equipe", EquipeController]);

    function EquipeController(Equipe) {
        var ctrl = this;

        ctrl.listaTimes = [];

        init();

        function init() {
            atualizaListaEquipes();
        }

        function atualizaListaEquipes() {
            Equipe.buscaListaEquipes().then(function (listaEquipes) {
                ctrl.listaEquipes = listaEquipes;
            });
        }
    }
})();