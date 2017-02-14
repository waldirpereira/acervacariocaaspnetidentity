(function () {
    "use strict";

    angular.module("acerva.regional")
        .factory("Regional", ["$http", "$q", "$timeout", "ROTAS", RegionalFactory]);

    function RegionalFactory($http, $q, $timeout, ROTAS) {
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
            salvaRegional: function (regional) {
                return $http.post(ROTAS.salva,
                    { regionalViewModel: regional }
                ).then(retornaDadoDoXhr);
            },
            anexaLogotipo: function (codigoRegional, arquivoAnexo) {
                var formData = new FormData();
                formData.append("codigoRegional", codigoRegional);
                formData.append("file", arquivoAnexo);

                var defer = $q.defer();
                $http.post(ROTAS.anexaLogotipo, formData,
                    {
                        withCredentials: true,
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
                    })
                .success(function (d) {
                    defer.resolve(d);
                })
                .error(function () {
                    defer.reject("Falha ao anexar arquivo!");
                });

                return defer.promise;
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