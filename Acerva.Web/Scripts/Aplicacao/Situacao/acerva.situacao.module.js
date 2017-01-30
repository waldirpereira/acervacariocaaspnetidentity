(function () {
    "use strict";

    angular.module("acerva.situacao", ["acerva"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "index.html",
                controller: "SituacaoController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();