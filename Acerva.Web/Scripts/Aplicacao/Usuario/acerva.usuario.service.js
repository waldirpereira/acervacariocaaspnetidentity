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
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            cancelaUsuario: function (usuario) {
                return $http.post(ROTAS.cancelaUsuario,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            reativaUsuario: function (usuario) {
                return $http.post(ROTAS.reativaUsuario,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            confirmaEmail: function (usuario) {
                return $http.post(ROTAS.confirmaEmail,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            confirmaIndicacao: function (usuario) {
                return $http.post(ROTAS.confirmaIndicacao,
                    { userId: usuario.id }
                ).then(retornaDadoDoXhr);
            },
            recusaIndicacao: function (usuario) {
                return $http.post(ROTAS.recusaIndicacao,
                    { userId: usuario.id }
                ).then(retornaDadoDoXhr);
            },
            confirmaPagamento: function (usuario) {
                return $http.post(ROTAS.confirmaPagamento,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            confirmaPagamentoSelecionados: function (idsUsuarios) {
                return $http.post(ROTAS.confirmaPagamentoSelecionados,
                    { idsUsuarios: idsUsuarios }
                ).then(retornaDadoDoXhr);
            },
            enviaEmailBoasVindasNaListaSelecionados: function (idsUsuarios) {
                return $http.post(ROTAS.enviaEmailBoasVindasNaListaSelecionados,
                    { idsUsuarios: idsUsuarios }
                ).then(retornaDadoDoXhr);
            },
            cobrancaGerada: function (usuario) {
                return $http.post(ROTAS.cobrancaGerada,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            cobrancaGeradaSelecionados: function (idsUsuarios) {
                return $http.post(ROTAS.cobrancaGeradaSelecionados,
                    { idsUsuarios: idsUsuarios }
                ).then(retornaDadoDoXhr);
            },
            voltarParaAguardandoConfirmacaoEmail: function (usuario) {
                return $http.post(ROTAS.voltarParaAguardandoConfirmacaoEmail,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            voltarParaAguardandoIndicacao: function (usuario) {
                return $http.post(ROTAS.voltarParaAguardandoIndicacao,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            voltaParaNovo: function (usuario) {
                return $http.post(ROTAS.voltaParaNovo,
                    { usuarioViewModel: usuario }
                ).then(retornaDadoDoXhr);
            },
            voltaParaAtivo: function (usuario) {
                return $http.post(ROTAS.voltaParaAtivo,
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