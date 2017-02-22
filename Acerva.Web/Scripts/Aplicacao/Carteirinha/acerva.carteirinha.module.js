(function () {
    "use strict";

    angular.module("acerva.carteirinha", ["acerva"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "CarteirinhaController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();