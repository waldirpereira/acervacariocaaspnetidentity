(function () {
    "use strict";

    angular.module("acerva.palpite", ["acerva", "datatables", "cfp.hotkeys"]);

    angular.module("acerva")
        .config(["$routeProvider", routes]);

    function routes($routeProvider) {
        $routeProvider
            .when("/:id", {
                templateUrl: "index.html",
                controller: "PalpiteController",
                controllerAs: "ctrl"
            })
            .when("/", {
                templateUrl: "index.html",
                controller: "PalpiteController",
                controllerAs: "ctrl"
            })
            .otherwise({
                redirectTo: "/"
            });
    }
})();