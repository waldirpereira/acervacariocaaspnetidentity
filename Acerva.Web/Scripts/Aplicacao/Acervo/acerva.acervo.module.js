(function () {
    "use strict";

    angular.module("acerva.acervo", ["acerva", "AngularPrint"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "AcervoController",
                controllerAs: "ctrl"
            })
            .when("/:codigoCategoria", {
                templateUrl: "index.html",
                controller: "AcervoController",
                controllerAs: "ctrl"
            })
            .when("/:codigoCategoria/:codigoArtigo", {
                templateUrl: "index.html",
                controller: "AcervoController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();