(function () {
    "use strict";

    angular.module("acerva.noticia")
        .controller("CadastroNoticiaController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Noticia", "$uibModal", CadastroNoticiaController]);

    function CadastroNoticiaController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Noticia, $uibModal) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};
        
        ctrl.salvaNoticia = salvaNoticia;
        ctrl.abrePopupAnexos = abrePopupAnexos;

        var idNoticia = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idNoticia);

        function init(id) {
            ctrl.status.carregando = true;

            Noticia.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaNoticiaEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Noticia.buscaNoticia(id).then(function(noticia) {
                    colocaNoticiaEmEdicao(noticia);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaNoticiaEmEdicao(noticia) {
            ctrl.status.bloqueado = noticia.codigo && !noticia.ativo;
            ctrl.modeloOriginal = noticia;
            ctrl.modelo = angular.copy(noticia);
        }

        function salvaNoticia() {
            if ($scope.formNoticia.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Noticia.salvaNoticia(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }

        function abrePopupAnexos(noticia) {
            Noticia.buscaAnexos(noticia.codigo)
                .then(function (anexos) {
                    $uibModal.open({
                        animation: true,
                        ariaLabelledBy: 'modal-title',
                        ariaDescribedBy: 'modal-body',
                        templateUrl: 'modal-anexos-noticia.html',
                        controller: 'NoticiaAnexosModalController',
                        controllerAs: 'ctrl',
                        resolve: {
                            noticia: noticia,
                            anexos: function () {
                                return anexos;
                            }
                        }
                    });
                });
        }
    }

})();