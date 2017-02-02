(function () {
    "use strict";

    angular.module("acerva.artigo")
        .factory("Artigo", ["$http", "$timeout", "ROTAS", ArtigoFactory]);

    function ArtigoFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaArtigos: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
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