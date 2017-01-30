(function () {
    "use strict";

    angular.module("acerva.usuario", ["acerva", "datatables", "LocalStorageModule", "naif.base64", "ngCroppie", "checklist-model"]);

    angular.module("acerva")
        .config(["$routeProvider", routes])
        .config(function (localStorageServiceProvider) {
            localStorageServiceProvider.setPrefix("acerva");
        });

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "CadastroUsuarioController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "CadastroUsuarioController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "UsuarioController",
                controllerAs: "listaCtrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();