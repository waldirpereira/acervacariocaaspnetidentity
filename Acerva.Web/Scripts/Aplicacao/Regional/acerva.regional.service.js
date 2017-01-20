(function () {
    "use strict";

    angular.module("acerva.regional")
        .factory("Regional", ["$http", "$timeout", "ROTAS", RegionalFactory]);

    function RegionalFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaRegionais: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaRegional: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            carregarPlacares: function (codigoRodada) {
                return $http.get(ROTAS.carregaPlacares, {
                    params: { codigoRodada: codigoRodada }
                }).then(retornaDadoDoXhr);
            },
            salvaRegional: function (regional) {
                return $http.post(ROTAS.salva,
                    { regionalViewModel: regional }
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
            ativa: function (regional) {
                return alteraAtivacao(regional, true);
            },
            inativa: function (regional) {
                return alteraAtivacao(regional, false);
            }
        }

        function alteraAtivacao(regional, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: regional.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();