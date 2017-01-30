(function () {
    "use strict";

    angular.module("acerva.situacao")
        .factory("Situacao", ["$http", "$timeout", "ROTAS", SituacaoFactory]);

    function SituacaoFactory($http, $timeout, ROTAS) {
        return {
            buscaSituacao: function (termo) {
                return $http.get(ROTAS.buscaSituacao, {
                    params: { termo: termo }
                }).then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();