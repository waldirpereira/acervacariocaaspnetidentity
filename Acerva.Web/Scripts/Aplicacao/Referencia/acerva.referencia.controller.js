(function () {
    "use strict";

    angular.module("acerva.referencia")
        .controller("ReferenciaController", ["Referencia", ReferenciaController]);

    function ReferenciaController(Referencia) {
        var ctrl = this;

        ctrl.status = 
        {
            carregandoArtigos: false,
            carregandoArtigo: false
        };

        ctrl.listaCategorias = [];
        ctrl.listaArtigosPorCategoria = [];
        ctrl.categoriaSelecionada = null;

        init();

        ctrl.selecionaCategoria = selecionaCategoria;
        ctrl.mostraArtigo = mostraArtigo;

        function init() {
            atualizaListaCategorias();
        }

        function atualizaListaCategorias() {
            Referencia.buscaCategorias().then(function (listaCategorias) {
                ctrl.listaCategorias = listaCategorias;
            });
        }

        function selecionaCategoria(categoria) {
            ctrl.categoriaSelecionada = categoria;
            ctrl.status.carregandoArtigos = true;
            ctrl.artigo = null;
            Referencia.buscaArtigosDaCategoria(categoria.codigo)
                .then(function (artigos) {
                    ctrl.listaArtigosPorCategoria[categoria.codigo] = artigos;
                })
                .finally(function () { ctrl.status.carregandoArtigos = false; });
        }

        function mostraArtigo(artigo) {
            ctrl.status.carregandoArtigo = true;
            Referencia.buscaArtigo(artigo.codigo)
                .then(function (artigo) {
                    ctrl.artigo = artigo;
                })
                .finally(function () { ctrl.status.carregandoArtigo = false; });
        }
    }
})();