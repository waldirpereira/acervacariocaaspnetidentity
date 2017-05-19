(function () {
    "use strict";

    angular.module("acerva.registro", ["acerva", "datatables", "naif.base64", "ngCpfCnpj", "ui.mask", "ngCropper"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Create", {
                templateUrl: "create.html",
                controller: "RegistroController",
                controllerAs: "ctrl"
            })
            .when("/ConfirmSent", {
                templateUrl: "confirmSent.html",
                controller: "RegistroController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/Create"
            });
    }
})();