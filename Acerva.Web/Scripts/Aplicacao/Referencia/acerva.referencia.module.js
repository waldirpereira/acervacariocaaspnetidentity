(function () {
    "use strict";

    angular.module("acerva.referencia", ["acerva"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "ReferenciaController",
                controllerAs: "ctrl"
            })
            .when("/:codigoCategoria", {
                templateUrl: "index.html",
                controller: "ReferenciaController",
                controllerAs: "ctrl"
            })
            .when("/:codigoCategoria/:codigoArtigo", {
                templateUrl: "index.html",
                controller: "ReferenciaController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();