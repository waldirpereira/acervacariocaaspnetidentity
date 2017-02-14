(function () {
    "use strict";

    angular.module("acerva.categoriaArtigo")
        .controller("CadastroCategoriaArtigoController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "CategoriaArtigo", CadastroCategoriaArtigoController]);

    function CadastroCategoriaArtigoController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, CategoriaArtigo) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.salvaCategoriaArtigo = salvaCategoriaArtigo;

        var idCategoriaArtigo = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idCategoriaArtigo);

        function init(id) {
            ctrl.status.carregando = true;

            if (id === 0) {
                colocaCategoriaArtigoEmEdicao({ ativo: true });
                ctrl.status.carregando = false;
                return;
            }

            return CategoriaArtigo.buscaCategoriaArtigo(id).then(function (categoriaArtigo) {
                colocaCategoriaArtigoEmEdicao(categoriaArtigo);
                ctrl.status.carregando = false;
            });
        }

        function colocaCategoriaArtigoEmEdicao(categoriaArtigo) {
            ctrl.status.bloqueado = categoriaArtigo.codigo && !categoriaArtigo.ativo;
            ctrl.modeloOriginal = categoriaArtigo;
            ctrl.modelo = angular.copy(categoriaArtigo);
        }

        function salvaCategoriaArtigo() {
            if ($scope.formCategoriaArtigo.$invalid)
                return;

            ctrl.status.salvando = true;
            
            CategoriaArtigo.salvaCategoriaArtigo(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location("#/");
                });
        }
    }

})();