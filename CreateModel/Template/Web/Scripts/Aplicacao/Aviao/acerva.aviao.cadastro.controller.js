(function () {
    "use strict";

    angular.module("acerva.aviao")
        .controller("CadastroAviaoController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Aviao", CadastroAviaoController]);

    function CadastroAviaoController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Aviao) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};
        
        ctrl.salvaAviao = salvaAviao;

        var idAviao = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idAviao);

        function init(id) {
            ctrl.status.carregando = true;

            Aviao.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaAviaoEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Aviao.buscaAviao(id).then(function(aviao) {
                    colocaAviaoEmEdicao(aviao);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaAviaoEmEdicao(aviao) {
            ctrl.status.bloqueado = aviao.codigo && !aviao.ativo;
            ctrl.modeloOriginal = aviao;
            ctrl.modelo = angular.copy(aviao);
        }

        function salvaAviao() {
            if ($scope.formAviao.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Aviao.salvaAviao(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }
    }

})();