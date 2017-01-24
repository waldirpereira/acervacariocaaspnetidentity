(function () {
    "use strict";

    angular.module("acerva.inicio")
        .controller("InicioController", ["Inicio", InicioController]);

    function InicioController(Inicio) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.listaNoticias = [];
        ctrl.indicacoesAConfirmar = [];

        init();

        function init() {
            ctrl.status.carregando = true;
            atualizaListaNoticias()
                .then(function() {
                    Inicio.buscaIndicacoesAConfirmar().then(function(indicacoes) {
                        ctrl.indicacoesAConfirmar = indicacoes.length;
                    });
                });
        }

        function atualizaListaNoticias() {
            return Inicio.buscaNoticias().then(function (noticias) {
                ctrl.listaNoticias = noticias;
                ctrl.status.carregando = false;
            });
        }
    }
})();