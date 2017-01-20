(function () {
    "use strict";

    angular.module("acerva.inicio", ["acerva", "datatables"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "InicioController",
                controllerAs: "listaCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();