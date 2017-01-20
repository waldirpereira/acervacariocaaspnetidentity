(function () {
    "use strict";

    angular.module("acerva")
         .factory("MensagemGrowlHttpInterceptor", ["$q", "CanalMensagemGrowl", MensagemGrowlHttpInterceptor])
         .factory("InsertAntiForgeryTokenInHeadersInterceptor", [InsertAntiForgeryTokenInHeadersInterceptor])
         .factory("ConvertDateStringsToDates", [ConvertDateStringsToDates])
         .service("CanalMensagemGrowl", ["$rootScope", CanalMensagemGrowl])
         .config(["$httpProvider", function ($httpProvider) {
             $httpProvider.interceptors.push("MensagemGrowlHttpInterceptor");
             $httpProvider.interceptors.push("InsertAntiForgeryTokenInHeadersInterceptor");
         }]);

    function MensagemGrowlHttpInterceptor($q, CanalMensagemGrowl) {
        // Baseado em http://bahmutov.calepin.co/catch-all-errors-in-angular-app.html
        
        function pegaMensagemDeErro(rejection) {
            var mensagem = rejection.statusText;

            var responseText = rejection.data;
            if (!responseText)
                return mensagem;

            var responseTextMin = rejection.data.toLowerCase();
            var inicioTitle = responseTextMin.indexOf("<title>");
            var fimTitle = responseTextMin.indexOf("</title>");
            if (inicioTitle < 0 || fimTitle < 0)
                return mensagem;

            inicioTitle += "<title>".length;

            mensagem = responseText.substring(inicioTitle, fimTitle);

            return mensagem;
        }

        return {
            response: function (response) {
                if (response.data && response.data.growlMessage) {
                    CanalMensagemGrowl.enviaNovaMensagem(response.data.growlMessage);
                }
                return response;
            },
            responseError: function responseError(rejection) {
                var mensagemGrowl;
                if (rejection.data && rejection.data.growlMessage) {
                    mensagemGrowl = rejection.data.growlMessage;
                } else {
                    var mensagemDeErro = pegaMensagemDeErro(rejection);
                    mensagemGrowl = {
                        message: rejection.statusText + "<br/>" + rejection.config.url + "<br/><br/>" + mensagemDeErro,
                        severity: "error",
                        config: {
                            title: "Erro na solicitação - status " + rejection.status
                        }
                    };
                }

                CanalMensagemGrowl.enviaNovaMensagem(mensagemGrowl);

                return $q.reject(rejection);
            }
        }
    }

    function InsertAntiForgeryTokenInHeadersInterceptor() {
        //http://www.jamienordmeyer.net/2015/09/04/anti-forgery-token-in-angular-apps/
        return {
            request: function (config) {
                if (config.method !== "POST") return config;

                var token = $("input:hidden[name=\"__RequestVerificationToken\"]").val();

                if (!!token && token.length > 0) {
                    if (!config.headers)
                        config.headers = { __RequestVerificationToken: token };
                    else
                        config.headers.__RequestVerificationToken = token;
                }

                return config;
            }
        };
    }

    function CanalMensagemGrowl($rootScope) {
        // baseado em http://eburley.github.io/2013/01/31/angularjs-watch-pub-sub-best-practices.html
        var NOVA_MENSAGEM = "novaMensagem";

        function enviaNovaMensagem(mensagem) {
            $rootScope.$broadcast(NOVA_MENSAGEM, mensagem);
        };

        function onNovaMensagem($scope, handler) {
            $scope.$on(NOVA_MENSAGEM, function (event, messagem) {
                handler(messagem);
            });
        };

        return {
            enviaNovaMensagem: enviaNovaMensagem,
            onNovaMensagem: onNovaMensagem
        };
    }

    //http://codepen.io/apuchkov/pen/JqCtL
    function ConvertDateStringsToDates() {
        var dateRegExMs = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})\.{1}\d{1,7}-\d{2}:\d{2}$/;
        var dateRegEx = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})$/;
        var dateRegExShort = /^(\d{4})-(\d{2})-(\d{2})$/;

        function tryMatchDate(dateString) {
            if (dateString.length == 19) {
                return dateString.match(dateRegEx);
            }
            if (dateString.length == 10) {
                return dateString.match(dateRegExShort);
            }
            if (dateString.length > 26 && dateString.length < 34) {
                return dateString.match(dateRegExMs);
            }
            return false;
        };


        function convertDateStringsToDates(input) {
            // Ignore things that aren't objects.
            if (typeof input !== "object")
                return input;

            for (var key in input) {
                if (!input.hasOwnProperty(key))
                    continue;

                var value = input[key];
                var match;

                // Check for string properties which look like dates.
                if (typeof value === "string" && (match = tryMatchDate(value))) {
                    var date = new Date(match[1], match[2] - 1, match[3], match[4] || 0, match[5] || 0, match[6] || 0);
                    input[key] = date;
                } else if (typeof value === "object") {
                    // Recurse into object
                    convertDateStringsToDates(value);
                }
            }
            return null;
        }

        return {
            response: function (response) {
                convertDateStringsToDates(response);
                return response;
            }
        };
    }
})();