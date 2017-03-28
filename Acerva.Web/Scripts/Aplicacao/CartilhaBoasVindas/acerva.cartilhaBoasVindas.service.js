(function () {
    "use strict";

    angular.module("acerva.cartilhaBoasVindas")
        .factory("CartilhaBoasVindas", ["$http", "$timeout", "ROTAS", CartilhaBoasVindasFactory]);

    function CartilhaBoasVindasFactory($http, $timeout, ROTAS) {
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