(function () {
    "use strict";

    angular.module("acerva.situacao")
        .factory("Situacao", ["$http", "$timeout", "ROTAS", SituacaoFactory]);

    function SituacaoFactory($http, $timeout, ROTAS) {
        return {
            buscaSituacao: function (cpf) {
                return $http.get(ROTAS.buscaSituacao, {
                    params: { cpf: cpf }
                }).then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();