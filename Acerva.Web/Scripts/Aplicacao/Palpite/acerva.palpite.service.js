(function () {
    "use strict";

    angular.module("acerva.palpite")
        .factory("Palpite", ["$http", "$timeout", "ROTAS", PalpiteFactory]);

    function PalpiteFactory($http, $timeout, ROTAS) {
        return {
            buscaParticipacao: function (codigoParticipacao) {
                return $http.post(ROTAS.buscaParticipacao,
                    { codigoParticipacao: codigoParticipacao }
                ).then(retornaDadoDoXhr);
            },
            salva: function (participacao) {
                return $http.post(ROTAS.salva,
                    { participacaoViewModel: participacao }
                ).then(retornaDadoDoXhr);
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();