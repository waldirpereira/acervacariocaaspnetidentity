(function () {
    "use strict";

    angular.module("acerva")
        .controller("LayoutController", ["$scope", "$rootScope", "$sce", "CanalMensagemGrowl", LayoutController]);

    function LayoutController($scope, $rootScope, $sce, CanalMensagemGrowl) {
        var ctrl = this;
        ctrl.mensagem = null;
        
        $rootScope.trustAsHtml = trustAsHtml;
        
        init();

        function init() {
            CanalMensagemGrowl.onNovaMensagem($scope, function (mensagem) {
                ctrl.mensagem = mensagem;
            });
        }

        function trustAsHtml(value) {
            return $sce.trustAsHtml(value);
        }
    }

})();