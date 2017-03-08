(function () {
    "use strict";

    angular.module("acerva.aviao")
        .factory("Aviao", ["$http", "$q", "$timeout", "ROTAS", AviaoFactory]);

    function AviaoFactory($http, $q, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaAvioes: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaAviao: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaAviao: function (aviao) {
                return $http.post(ROTAS.salva,
                    { aviaoViewModel: aviao }
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
            },
            ativa: function (aviao) {
                return alteraAtivacao(aviao, true);
            },
            inativa: function (aviao) {
                return alteraAtivacao(aviao, false);
            }
        }

        function alteraAtivacao(aviao, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: aviao.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();