(function () {
    "use strict";

    angular.module("acerva.registro", ["acerva", "datatables", "naif.base64", "ngCroppie", "ngCpfCnpj", "ui.mask"]);

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