(function () {
    "use strict";

    angular.module("acerva.usuario", ["acerva", "datatables", "naif.base64", "ngCroppie", "checklist-model", "datatables.buttons", "ngCpfCnpj", "ui.mask"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

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