(function () {
    "use strict";

    angular.module("acerva.categoriaArtigo", ["acerva", "datatables"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "CadastroCategoriaArtigoController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "CadastroCategoriaArtigoController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "CategoriaArtigoController",
                controllerAs: "listaCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();