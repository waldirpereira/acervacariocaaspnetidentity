(function () {
    "use strict";

    angular.module("acerva.referencia")
        .factory("Referencia", ["$http", "$timeout", "ROTAS", ReferenciaFactory]);

    function ReferenciaFactory($http, $timeout, ROTAS) {
        return {
            buscaCategorias: function () {
                return $http.get(ROTAS.buscaCategorias)
                    .then(retornaDadoDoXhr);
            },
            buscaArtigosDaCategoria: function (codigoCategoriaArtigo) {
                return $http.get(ROTAS.buscaArtigosDaCategoria, {
                    params: { codigoCategoriaArtigo: codigoCategoriaArtigo }
                }).then(retornaDadoDoXhr);
            },
            buscaArtigo: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();