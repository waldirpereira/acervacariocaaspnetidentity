(function () {
    "use strict";

    angular.module("acerva.artigo", ["acerva", "datatables"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "CadastroArtigoController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "CadastroArtigoController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "ArtigoController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();