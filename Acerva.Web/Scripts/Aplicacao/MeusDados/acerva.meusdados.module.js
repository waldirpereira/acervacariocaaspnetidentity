(function () {
    "use strict";

    angular.module("acerva.meusdados", ["acerva", "naif.base64", "ngCroppie", "ngCpfCnpj", "ui.mask"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit", {
                templateUrl: "edit.html",
                controller: "MeusDadosController",
                controllerAs: "ctrl"
            })
            .when("/Edited", {
                templateUrl: "edited.html",
                controller: "MeusDadosController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/Edit"
            });
    }
})();