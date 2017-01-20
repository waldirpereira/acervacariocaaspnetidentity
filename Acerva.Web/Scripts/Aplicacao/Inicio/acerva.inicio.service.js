(function () {
    "use strict";

    angular.module("acerva.inicio")
        .factory("Inicio", ["$http", "$timeout", "ROTAS", InicioFactory]);

    function InicioFactory($http, $timeout, ROTAS) {
        return {
            buscaListaMinhasAcervas: function () {
                return $http.get(ROTAS.buscaTodosMinhasAcervas)
                    .then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();