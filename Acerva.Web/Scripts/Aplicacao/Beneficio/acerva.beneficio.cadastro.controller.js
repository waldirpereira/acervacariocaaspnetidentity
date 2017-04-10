(function () {
    "use strict";

    angular.module("acerva.beneficio")
        .controller("CadastroBeneficioController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Beneficio", CadastroBeneficioController]);

    function CadastroBeneficioController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Beneficio) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};
        
        ctrl.salvaBeneficio = salvaBeneficio;

        var idBeneficio = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idBeneficio);

        function init(id) {
            ctrl.status.carregando = true;

            Beneficio.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaBeneficioEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Beneficio.buscaBeneficio(id).then(function(beneficio) {
                    colocaBeneficioEmEdicao(beneficio);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaBeneficioEmEdicao(beneficio) {
            ctrl.status.bloqueado = beneficio.codigo && !beneficio.ativo;
            ctrl.modeloOriginal = beneficio;
            ctrl.modelo = angular.copy(beneficio);
        }

        function salvaBeneficio() {
            if ($scope.formBeneficio.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Beneficio.salvaBeneficio(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }
    }

})();