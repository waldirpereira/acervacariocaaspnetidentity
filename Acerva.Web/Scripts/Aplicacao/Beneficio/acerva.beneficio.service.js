(function () {
    "use strict";

    angular.module("acerva.beneficio")
        .factory("Beneficio", ["$http", "$q", "$timeout", "ROTAS", BeneficioFactory]);

    function BeneficioFactory($http, $q, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaBeneficios: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaBeneficio: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaBeneficio: function (beneficio) {
                return $http.post(ROTAS.salva,
                    { beneficioViewModel: beneficio }
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
            ativa: function (beneficio) {
                return alteraAtivacao(beneficio, true);
            },
            inativa: function (beneficio) {
                return alteraAtivacao(beneficio, false);
            }
        }

        function alteraAtivacao(beneficio, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: beneficio.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();