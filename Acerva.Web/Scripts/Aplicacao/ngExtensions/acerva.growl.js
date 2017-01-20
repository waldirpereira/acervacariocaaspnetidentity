(function () {
    "use strict";

    angular.module("acerva.growl", ['angular-growl']);

    angular.module("acerva.growl")
        .config(['growlProvider', function (growlProvider) {
            growlProvider.globalTimeToLive({ success: 5000, error: -1, warning: 10000, info: 5000 });
        }])
        .directive("acervaGrowl", ["growl", function (growl) {
            return {
                restrict: "E",
                scope: {
                    alerta: "="
                },
                template: "<div growl></div>",
                link: link
            };

            function link(scope) {
                scope.$watch("alerta", function (alerta) {
                    if (!alerta) {
                        return;
                    }
                    var message = alerta.message;

                    if (alerta.details) {
                        message = message + "<ul><li>" + alerta.details.join(";</li><li>") + ".</li></ul>";
                    }

                    growl.general(message, alerta.config, alerta.severity);
                });
            }
        }
        ]);

})();