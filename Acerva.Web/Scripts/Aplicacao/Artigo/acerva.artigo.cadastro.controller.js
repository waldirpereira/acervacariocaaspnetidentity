(function () {
    "use strict";

    angular.module("acerva.artigo")
        .controller("CadastroArtigoController", ["$scope", "$timeout", "$routeParams", "$location", "ENUMS", "CanalMensagemGrowl", "Artigo", "$uibModal", CadastroArtigoController]);

    function CadastroArtigoController($scope, $timeout, $routeParams, $location, ENUMS, CanalMensagemGrowl, Artigo, $uibModal) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.salvaArtigo = salvaArtigo;
        ctrl.abrePopupAnexos = abrePopupAnexos;

        var idArtigo = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idArtigo);

        function init(id) {
            ctrl.status.carregando = true;

            Artigo.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.visibilidade = ENUMS.visibilidadeArtigo;
                ctrl.dominio.visibilidades = ENUMS.toArrayOfEnums(ENUMS.visibilidadeArtigo);

                if (id === 0) {
                    colocaArtigoEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Artigo.buscaArtigo(id).then(function(artigo) {
                    colocaArtigoEmEdicao(artigo);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaArtigoEmEdicao(artigo) {
            ctrl.status.bloqueado = artigo.codigo && !artigo.ativo;
            ctrl.modeloOriginal = artigo;
            ctrl.modelo = angular.copy(artigo);
        }

        function salvaArtigo() {
            if ($scope.formArtigo.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Artigo.salvaArtigo(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }

        function abrePopupAnexos(artigo) {
            Artigo.buscaAnexos(artigo.codigo)
                .then(function (anexos) {
                    $uibModal.open({
                        animation: true,
                        ariaLabelledBy: 'modal-title',
                        ariaDescribedBy: 'modal-body',
                        templateUrl: 'modal-anexos-artigo.html',
                        controller: 'ArtigoAnexosModalController',
                        controllerAs: 'ctrl',
                        resolve: {
                            artigo: artigo,
                            anexos: function () {
                                return anexos;
                            }
                        }
                    });
                });
        }
    }

})();