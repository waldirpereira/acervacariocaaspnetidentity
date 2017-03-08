(function () {
    "use strict";

    angular.module("acerva.aviao")
        .controller("AviaoController", ["Aviao", AviaoController]);

    function AviaoController(Aviao) {
        var ctrl = this;

        ctrl.listaAvioes = [];

        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaAvioes();
        }

        function atualizaListaAvioes() {
            Aviao.buscaListaAvioes().then(function (listaAvioes) {
                ctrl.listaAvioes = listaAvioes;
            });
        }

        function ativa(aviao) {
            Aviao.ativa(aviao).then(function () {
                aviao.ativo = true;
            });
        }

        function inativa(aviao) {
            Aviao.inativa(aviao).then(function () {
                aviao.ativo = false;
            });
        }
    }
})();