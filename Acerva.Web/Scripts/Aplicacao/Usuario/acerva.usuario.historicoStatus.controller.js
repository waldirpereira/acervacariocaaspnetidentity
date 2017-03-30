(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("HistoricoStatusModalController", ["$scope", "$uibModalInstance", "usuario", "dominio", "listaHistorico", HistoricoStatusModalController]);

    function HistoricoStatusModalController($scope, $uibModalInstance, usuario, dominio, listaHistorico) {
        var ctrl = this;

        ctrl.modelo = {};

        ctrl.usuario = usuario;
        ctrl.dominio = dominio;
        ctrl.listaHistorico = listaHistorico;

        ctrl.ok = ok;

        function ok() {
            $uibModalInstance.close();
        }
    }

})();