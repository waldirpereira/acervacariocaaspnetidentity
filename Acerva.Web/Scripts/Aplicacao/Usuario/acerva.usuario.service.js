(function () {
    "use strict";

    angular.module("acerva.usuario")
        .factory("Usuario", ["$http", "$timeout", "ROTAS", UsuarioFactory]);

    function UsuarioFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaUsuarios: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaUsuario: function (id) {
                return $http.get(ROTAS.busca, {
                    params: { id: id }
                }).then(retornaDadoDoXhr);
            },
            salvaUsuario: function (usuario) {
                return $http.post(ROTAS.salva,
                    { userViewModel: usuario }
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
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();