(function () {
    "use strict";

    angular.module("acerva.registro", ["acerva", "datatables"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/Edit/:id", {
                templateUrl: "edit.html",
                controller: "RegistroController",
                controllerAs: "ctrl"
            })
            .when("/Create", {
                templateUrl: "edit.html",
                controller: "RegistroController",
                controllerAs: "ctrl"
            })
            .when("/ConfirmSent", {
                templateUrl: "confirmSent.html",
                controller: "RegistroController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/Create"
            });
    }
})();