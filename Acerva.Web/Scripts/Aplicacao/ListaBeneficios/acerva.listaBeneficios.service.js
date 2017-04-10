(function () {
    "use strict";

    angular.module("acerva.listaBeneficios")
        .factory("ListaBeneficios", ["$http", "ROTAS", ListaBeneficiosFactory]);

    function ListaBeneficiosFactory($http, ROTAS) {
        return {
            buscaBeneficios: function () {
                return $http.get(ROTAS.buscaBeneficios)
                    .then(retornaDadoDoXhr);
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();