(function () {
    "use strict";

    angular.module("acerva.artigo")
        .factory("Artigo", ["$http", "$q", "$timeout", "ROTAS", ArtigoFactory]);

    function ArtigoFactory($http, $q, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaArtigos: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaAnexos: function (codigoArtigo) {
                return $http.get(ROTAS.buscaAnexos, {
                    params: { codigoArtigo: codigoArtigo }
                }).then(retornaDadoDoXhr);
            },
            buscaArtigo: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaArtigo: function (artigo) {
                return $http.post(ROTAS.salva,
                    { artigoViewModel: artigo }
                ).then(retornaDadoDoXhr);
            },
            excluiAnexo: function (codigoAnexo) {
                return $http.post(ROTAS.excluiAnexo,
                    { codigoAnexo: codigoAnexo }
                ).then(retornaDadoDoXhr);
            },
            salvaAnexo: function(codigoArtigo, arquivoAnexo, tituloAnexo) {
                var formData = new FormData();
                formData.append("codigoArtigo", codigoArtigo);
                formData.append("titulo", tituloAnexo);
                formData.append("file", arquivoAnexo);
                
                var defer = $q.defer();
                $http.post(ROTAS.salvaAnexo, formData,
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
            ativa: function (artigo) {
                return alteraAtivacao(artigo, true);
            },
            inativa: function (artigo) {
                return alteraAtivacao(artigo, false);
            }
        }

        function alteraAtivacao(artigo, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: artigo.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();