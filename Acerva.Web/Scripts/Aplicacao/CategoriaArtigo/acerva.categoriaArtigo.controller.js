(function () {
    "use strict";

    angular.module("acerva.categoriaArtigo")
        .controller("CategoriaArtigoController", ["CategoriaArtigo", CategoriaArtigoController]);

    function CategoriaArtigoController(CategoriaArtigo) {
        var ctrl = this;

        ctrl.listaCategoriasArtigos = [];
        
        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaCategoriasArtigos();
        }

        function atualizaListaCategoriasArtigos() {
            CategoriaArtigo.buscaListaCategoriasArtigos().then(function (listaCategoriasArtigos) {
                ctrl.listaCategoriasArtigos = listaCategoriasArtigos;
            });
        }

         function ativa(categoriaArtigo) {
            CategoriaArtigo.ativa(categoriaArtigo).then(function () {
                categoriaArtigo.ativo = true;
            });
        }

        function inativa(categoriaArtigo) {
            CategoriaArtigo.inativa(categoriaArtigo).then(function () {
                categoriaArtigo.ativo = false;
            });
        }
    }
})();