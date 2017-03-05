(function () {
    "use strict";

    angular.module("acerva.votacao")
        .controller("CadastroVotacaoController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "Votacao", CadastroVotacaoController]);

    function CadastroVotacaoController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, Votacao) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};
        
        ctrl.salvaVotacao = salvaVotacao;

        var idVotacao = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idVotacao);

        function init(id) {
            ctrl.status.carregando = true;

            Votacao.buscaTiposDominio().then(function(tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaVotacaoEmEdicao({ativo: true});
                    ctrl.status.carregando = false;
                    return;
                }

                return Votacao.buscaVotacao(id).then(function(votacao) {
                    colocaVotacaoEmEdicao(votacao);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaVotacaoEmEdicao(votacao) {
            ctrl.status.bloqueado = votacao.codigo && !votacao.ativo;
            ctrl.modeloOriginal = votacao;
            ctrl.modelo = angular.copy(votacao);
        }

        function salvaVotacao() {
            if ($scope.formVotacao.$invalid)
                return;

            ctrl.status.salvando = true;
            
            Votacao.salvaVotacao(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }
    }

})();