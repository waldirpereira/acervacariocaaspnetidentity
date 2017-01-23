(function () {
    "use strict";

    angular.module("acerva.registro")
        .factory("Registro", ["$http", "$timeout", "ROTAS", RegistroFactory]);

    function RegistroFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaUsuario: function (id) {
                return $http.get(ROTAS.busca, {
                    params: { id: id }
                }).then(retornaDadoDoXhr);
            },
            salvaUsuario: function (usuario) {
                return $http.post(ROTAS.salva,
                    { usuarioViewModel: usuario }
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
            buscaUsuariosAtivosComTermo: function(termo) {
                return $http.get(ROTAS.buscaUsuariosAtivosComTermo, {
                    params: { termo: termo }
                }).then(retornaDadoDoXhr);
            }
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();