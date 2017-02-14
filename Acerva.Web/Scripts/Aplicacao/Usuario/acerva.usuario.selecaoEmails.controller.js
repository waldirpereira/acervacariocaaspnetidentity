(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("SelecaoEmailsModalController", ["$scope", "$uibModalInstance", SelecaoEmailsModalController]);

    function SelecaoEmailsModalController($scope, $uibModalInstance) {
        var ctrl = this;

        ctrl.modelo = {};

        ctrl.ok = ok;

        function ok() {
            $uibModalInstance.close(ctrl.modelo.emails);
        }
    }

})();