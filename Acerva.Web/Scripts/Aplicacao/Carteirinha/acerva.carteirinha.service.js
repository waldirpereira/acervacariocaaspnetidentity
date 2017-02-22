(function () {
    "use strict";

    angular.module("acerva.carteirinha")
        .factory("Carteirinha", ["$http", "$timeout", "ROTAS", CarteirinhaFactory]);

    function CarteirinhaFactory($http, $timeout, ROTAS) {
        return {
            busca: function () {
                return $http.get(ROTAS.busca, {
                    params: { }
                }).then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();