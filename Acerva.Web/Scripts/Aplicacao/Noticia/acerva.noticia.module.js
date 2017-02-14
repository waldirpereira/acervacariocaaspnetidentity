(function () {
    "use strict";

    angular.module("acerva.regional", ["acerva", "datatables", "textAngular"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "CadastroRegionalController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "CadastroRegionalController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "RegionalController",
                controllerAs: "listaCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();