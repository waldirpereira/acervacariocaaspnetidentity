(function () {
    "use strict";

    angular.module("acerva.regional")
        .controller("CadastroRegionalController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Regional", CadastroRegionalController]);

    function CadastroRegionalController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Regional) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        $scope.selecionaArquivoParaUpload = function (file) {
            ctrl.modelo.arquivo = file[0];
        }

        ctrl.salvaRegional = salvaRegional;
        ctrl.anexaLogotipo = anexaLogotipo;

        var idRegional = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idRegional);

        function init(id) {
            ctrl.status.carregando = true;

            Regional.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaRegionalEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Regional.buscaRegional(id).then(function(regional) {
                    colocaRegionalEmEdicao(regional);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaRegionalEmEdicao(regional) {
            ctrl.status.bloqueado = regional.codigo && !regional.ativo;
            ctrl.modeloOriginal = regional;
            ctrl.modelo = angular.copy(regional);
        }

        function salvaRegional() {
            if ($scope.formRegional.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Regional.salvaRegional(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }

        function anexaLogotipo() {
            Regional.anexaLogotipo(ctrl.modelo.codigo, ctrl.modelo.arquivo)
                .then(function() {
                    ctrl.modelo.nomeArquivoLogo = ctrl.modelo.arquivo.name;
                });
        }
    }

})();