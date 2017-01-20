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

        init();

        function init() {
            ctrl.status.carregando = true;
            atualizaListaNoticias();
        }

        function atualizaListaNoticias() {
            Inicio.buscaNoticias().then(function (noticias) {
                ctrl.listaNoticias = noticias;
                ctrl.status.carregando = false;
            });
        }
    }
})();