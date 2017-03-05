(function () {
    "use strict";

    angular.module("acerva.votacao")
        .factory("Votacao", ["$http", "$q", "$timeout", "ROTAS", VotacaoFactory]);

    function VotacaoFactory($http, $q, $timeout, ROTAS) {
        var cacheTiposDominio;

        return {
            buscaListaVotacoes: function () {
                return $http.get(ROTAS.buscaTodos)
                    .then(retornaDadoDoXhr);
            },
            buscaVotacao: function (codigo) {
                return $http.get(ROTAS.busca, {
                    params: { codigo: codigo }
                }).then(retornaDadoDoXhr);
            },
            salvaVotacao: function (votacao) {
                return $http.post(ROTAS.salva,
                    { votacaoViewModel: votacao }
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
            ativa: function (votacao) {
                return alteraAtivacao(votacao, true);
            },
            inativa: function (votacao) {
                return alteraAtivacao(votacao, false);
            }
        }

        function alteraAtivacao(votacao, ativo) {
            return $http.post(ROTAS.alteraAtivacao,
                { id: votacao.codigo, ativo: ativo }
            ).then(function (response) {
                return response.data;
            });
        }
        
        function retornaDadoDoXhr(response) {
            return response.data;
        }
    }
})();