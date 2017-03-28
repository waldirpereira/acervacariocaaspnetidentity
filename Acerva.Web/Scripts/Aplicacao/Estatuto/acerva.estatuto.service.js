(function () {
    "use strict";

    angular.module("acerva.estatuto")
        .factory("Estatuto", ["$http", "$timeout", "ROTAS", EstatutoFactory]);

    function EstatutoFactory($http, $timeout, ROTAS) {
        return {
            busca: function () {
                return $http.post(ROTAS.busca)
                    .then(retornaDadoDoXhr);
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();