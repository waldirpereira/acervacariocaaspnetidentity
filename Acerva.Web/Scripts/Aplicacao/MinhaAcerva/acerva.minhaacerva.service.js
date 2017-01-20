(function () {
    "use strict";

    angular.module("acerva.minhaAcerva")
        .factory("MinhaAcerva", ["$http", "$timeout", "ROTAS", MinhaAcervaFactory]);

    function MinhaAcervaFactory($http, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaMinhasAcervas: function () {
                return $http.get(ROTAS.buscaTodosMinhasAcervas)
                    .then(retornaDadoDoXhr);
            },
            buscaMinhaAcerva: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaMinhaAcerva: function (minhaAcerva) {
                return $http.post(ROTAS.salvaMinhaAcerva,
                    { acervaViewModel: minhaAcerva }
                ).then(retornaDadoDoXhr);
            },
            reenviaConviteParticipacao: function (participacao) {
                return $http.post(ROTAS.reenviaConviteParticipacao,
                    { participacaoViewModel: participacao }
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
            ativa: function (acerva) {
                return alteraAtivacao(acerva, true);
            },
            inativa: function (acerva) {
                return alteraAtivacao(acerva, false);
            },
            buscaEvolucao: function (codigo) {
                return $http.get(ROTAS.buscaEvolucao, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            }
        }
        
        function alteraAtivacao(acerva, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: acerva.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }

        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();