(function () {
    "use strict";

    angular.module("acerva.noticia")
        .factory("Noticia", ["$http", "$q", "$timeout", "ROTAS", NoticiaFactory]);

    function NoticiaFactory($http, $q, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaNoticias: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaNoticia: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaNoticia: function (noticia) {
                return $http.post(ROTAS.salva,
                    { noticiaViewModel: noticia }
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
            ativa: function (noticia) {
                return alteraAtivacao(noticia, true);
            },
            inativa: function (noticia) {
                return alteraAtivacao(noticia, false);
            }
        }

        function alteraAtivacao(noticia, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: noticia.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();