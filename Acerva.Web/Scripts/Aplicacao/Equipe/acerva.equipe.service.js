(function () {
    "use strict";

    angular.module("acerva.equipe")
        .factory("Equipe", ["$http", "$timeout", "ROTAS", EquipeFactory]);

    function EquipeFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaEquipes: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaEquipe: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaEquipe: function (equipe) {
                return $http.post(ROTAS.salva,
                    { equipeViewModel: equipe }
                ).then(retornaDadoDoXhr);
            },
            buscaTiposDominio: function () {
                if (cacheTiposDominio) {
                    return $timeout(function () { return cacheTiposDominio; });
                }
                return $http.get(ROTAS.buscaTiposDominio)
                    .then(retornaDadoDoXhr)
                    .then(function (tiposDominio) {
                        cacheTiposDominio = tiposDominio;
                        return tiposDominio;
                    });
            }
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();