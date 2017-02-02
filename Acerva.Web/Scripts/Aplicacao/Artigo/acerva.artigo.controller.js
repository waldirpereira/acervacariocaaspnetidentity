(function () {
    "use strict";

    angular.module("acerva.artigo")
        .controller("ArtigoController", ["Artigo", ArtigoController]);

    function ArtigoController(Artigo) {
        var ctrl = this;

        ctrl.buscaListaArtigos = [];
        
        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaArtigos();
        }

        function atualizaListaArtigos() {
            Artigo.buscaListaArtigos().then(function (listaArtigos) {
                ctrl.listaArtigos = listaArtigos;
            });
        }

         function ativa(artigo) {
            Artigo.ativa(artigo).then(function () {
                artigo.ativo = true;
            });
        }

        function inativa(artigo) {
            Artigo.inativa(artigo).then(function () {
                artigo.ativo = false;
            });
        }
    }
})();