(function () {
    "use strict";

    angular.module("acerva.meusdados")
        .factory("MeusDados", ["$http", "$timeout", "ROTAS", MeusDadosFactory]);

    function MeusDadosFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaUsuarioLogadoParaEdicao: function () {
                return $http.get(ROTAS.buscaUsuarioLogadoParaEdicao, {
                    params: { }
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