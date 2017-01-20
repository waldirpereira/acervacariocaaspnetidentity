(function () {
    "use strict";

    angular.module("acerva.equipe", ["acerva", "datatables"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "CadastroEquipeController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "CadastroEquipeController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "EquipeController",
                controllerAs: "listaCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();