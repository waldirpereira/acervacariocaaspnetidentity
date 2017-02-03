(function () {
    "use strict";

    angular.module("acerva.listaRegionais")
        .factory("ListaRegionais", ["$http", "ROTAS", ListaRegionaisFactory]);

    function ListaRegionaisFactory($http, ROTAS) {
        return {
            buscaRegionais: function () {
                return $http.get(ROTAS.buscaRegionais)
                    .then(retornaDadoDoXhr);
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();