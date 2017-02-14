(function () {
    "use strict";

    angular.module("acerva.categoriaArtigo")
        .factory("CategoriaArtigo", ["$http", "$q", "$timeout", "ROTAS", CategoriaArtigoFactory]);

    function CategoriaArtigoFactory($http, $q, $timeout, ROTAS) {

        return {
            buscaListaCategoriasArtigos: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaCategoriaArtigo: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaCategoriaArtigo: function (categoriaArtigo) {
                return $http.post(ROTAS.salva,
                    { categoriaArtigoViewModel: categoriaArtigo }
                ).then(retornaDadoDoXhr);
            },
            ativa: function (categoriaArtigo) {
                return alteraAtivacao(categoriaArtigo, true);
            },
            inativa: function (categoriaArtigo) {
                return alteraAtivacao(categoriaArtigo, false);
            }
        }

        function alteraAtivacao(categoriaArtigo, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: categoriaArtigo.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();