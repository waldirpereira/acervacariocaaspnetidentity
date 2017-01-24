(function () {
    "use strict";

    angular.module("acerva.inicio")
        .factory("Inicio", ["$http", "$timeout", "ROTAS", InicioFactory]);

    function InicioFactory($http, $timeout, ROTAS) {
        return {
            buscaNoticias: function () {
                return $http.get(ROTAS.buscaNoticias)
                    .then(retornaDadoDoXhr);
            },
            buscaIndicacoesAConfirmar: function () {
                return $http.get(ROTAS.buscaIndicacoesAConfirmar)
                    .then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();