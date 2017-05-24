(function () {
    "use strict";

    angular.module("acerva.acervo")
        .controller("AcervoController", ["$timeout", "$routeParams", "$location", "ENUMS", "Acervo", AcervoController]);

    function AcervoController($timeout, $routeParams, $location, ENUMS, Acervo) {
        var ctrl = this;

        ctrl.dominio = {};

        ctrl.status =
        {
            carregandoArtigos: false,
            carregandoArtigo: false
        };

        ctrl.listaCategorias = [];
        ctrl.listaArtigosPorCategoria = [];
        ctrl.categoriaSelecionada = null;

        var codigoCategoria = $routeParams.codigoCategoria ? +$routeParams.codigoCategoria : null;
        var codigoArtigo = $routeParams.codigoArtigo ? +$routeParams.codigoArtigo : null;
        init(codigoCategoria, codigoArtigo);

        ctrl.selecionaCategoria = selecionaCategoria;
        ctrl.mostraArtigo = mostraArtigo;

        function init() {
            atualizaListaCategorias();
        }

        function atualizaListaCategorias() {
            Acervo.buscaCategorias().then(function (listaCategorias) {

                ctrl.dominio.visibilidade = ENUMS.visibilidadeArtigo;

                ctrl.listaCategorias = listaCategorias;

                if (!ctrl.categoriaSelecionada && listaCategorias.length) {
                    if (!codigoCategoria) {
                        codigoCategoria = listaCategorias[0].codigo;
                        $location.path("/" + codigoCategoria);
                    }

                    var indexOfCategoria = listaCategorias.map(function (categoria) { return categoria.codigo; })
                        .indexOf(codigoCategoria);

                    var categoriaSelecionada = listaCategorias[indexOfCategoria];
                    
                    return selecionaCategoria(categoriaSelecionada).then(function () {
                        if (!codigoArtigo)
                            return;

                        var indexOfArtigo = ctrl.listaArtigosPorCategoria[codigoCategoria]
                            .map(function (artigo) { return artigo.codigo; }).indexOf(codigoArtigo);

                        if (indexOfArtigo < 0)
                            return null;

                        var artigo = ctrl.listaArtigosPorCategoria[codigoCategoria][indexOfArtigo];
                        return mostraArtigo(artigo);
                    });
                }
            });
        }

        function selecionaCategoria(categoria) {
            ctrl.categoriaSelecionada = categoria;
            ctrl.status.carregandoArtigos = true;
            ctrl.artigo = null;

            if (!categoria)
                return $timeout();

            return Acervo.buscaArtigosDaCategoria(categoria.codigo)
                .then(function (artigos) {
                    ctrl.listaArtigosPorCategoria[categoria.codigo] = artigos;
                })
                .finally(function () { ctrl.status.carregandoArtigos = false; });
        }

        function mostraArtigo(artigo) {
            ctrl.status.carregandoArtigo = true;
            Acervo.buscaArtigo(artigo.codigo)
                .then(function (artigo) {
                    ctrl.artigo = artigo;
                })
                .finally(function () { ctrl.status.carregandoArtigo = false; });
        }
    }
})();