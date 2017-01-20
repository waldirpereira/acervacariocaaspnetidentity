(function () {
    "use strict";

    angular.module("acerva.equipe")
        .controller("CadastroEquipeController", ["$scope", "$routeParams", "$location", "Equipe", CadastroEquipeController]);

    function CadastroEquipeController($scope, $routeParams, $location, Equipe) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.salvaEquipe = salvaEquipe;

        var idEquipe = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idEquipe);

        function init(id) {
            ctrl.status.carregando = true;

            Equipe.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaEquipeEmEdicao({});
                    ctrl.status.carregando = false;
                    return;
                }

                return Equipe.buscaEquipe(id).then(function(equipe) {
                    colocaEquipeEmEdicao(equipe);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaEquipeEmEdicao(equipe) {
            ctrl.modeloOriginal = equipe;
            ctrl.modelo = angular.copy(equipe);
        }

        function salvaEquipe() {
            if ($scope.formEquipe.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Equipe.salvaEquipe(ctrl.modelo)
                .then(function () { $location.path("/"); })
                .finally(function () { ctrl.status.salvando = false; });
        }
    }

})();