(function () {
    "use strict";

    angular.module("acerva.listaRegionais", ["acerva"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "ListaRegionaisController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();