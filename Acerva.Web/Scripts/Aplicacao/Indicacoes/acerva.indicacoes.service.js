(function () {
    "use strict";

    angular.module("acerva.indicacoes")
        .factory("Indicacoes", ["$http", "$timeout", "ROTAS", IndicacoesFactory]);

    function IndicacoesFactory($http, $timeout, ROTAS) {
        return {
            buscaIndicacoesAConfirmar: function () {
                return $http.get(ROTAS.buscaIndicacoesAConfirmar)
                    .then(retornaDadoDoXhr);
            },
            confirmaIndicacao: function (userId) {
                return $http.post(ROTAS.confirmaIndicacao,
                    { userId: userId }
                ).then(function (response) {
                    return response.data;
                });
            },
            declinaIndicacao: function (userId) {
                return $http.post(ROTAS.declinaIndicacao,
                    { userId: userId }
                ).then(function (response) {
                    return response.data;
                });
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();