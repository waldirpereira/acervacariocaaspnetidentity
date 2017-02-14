(function () {
    "use strict";

    angular.module("acerva.noticia")
        .controller("NoticiaController", ["Noticia", NoticiaController]);

    function NoticiaController(Noticia) {
        var ctrl = this;

        ctrl.listaNoticias = [];

        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaNoticias();
        }

        function atualizaListaNoticias() {
            Noticia.buscaListaNoticias().then(function (listaNoticias) {
                ctrl.listaNoticias = listaNoticias;
            });
        }

        function ativa(noticia) {
            Noticia.ativa(noticia).then(function () {
                noticia.ativo = true;
            });
        }

        function inativa(noticia) {
            Noticia.inativa(noticia).then(function () {
                noticia.ativo = false;
            });
        }
    }
})();