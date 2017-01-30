(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["$scope", "Usuario", "ENUMS", "DTOptionsBuilder", "localStorageService", UsuarioController]);

    function UsuarioController($scope, Usuario, ENUMS, DTOptionsBuilder, localStorageService) {
        var ctrl = this;

        ctrl.listaUsuarios = [];
        ctrl.usuariosSelecionados = [];
        ctrl.todosFiltrados = false;

        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };

        var filtroStatusKey = "usuario.filtroStatus";
        ctrl.filtroStatus = [];

        atualizaFiltrosDaLocalStorage();
        
        ctrl.selecionaTodosFiltrados = selecionaTodosFiltrados;
        ctrl.mudaFiltroStatus = mudaFiltroStatus;
        ctrl.alteraSelecao = alteraSelecao;
        ctrl.estaFiltradoPorStatus = estaFiltradoPorStatus;
        ctrl.pegaUsuariosFiltrados = pegaUsuariosFiltrados;
        ctrl.confirmaPagamentoSelecionados = confirmaPagamentoSelecionados;
        ctrl.cobrancaGeradaSelecionados = cobrancaGeradaSelecionados;

        ctrl.dtOptions = DTOptionsBuilder.newOptions()
            .withOption('order', [2, 'asc']);

        init();

        function init() {
            Usuario.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;

                return atualizaListaUsuarios();
            });
        }

        function atualizaListaUsuarios() {
            return Usuario.buscaListaUsuarios().then(function (listaUsuarios) {
                ctrl.listaUsuarios = listaUsuarios;
            });
        }

        function selecionaTodosFiltrados() {
            ctrl.usuariosSelecionados = [];
            if (!ctrl.todosFiltrados)
                return;

            ctrl.pegaUsuariosFiltrados().forEach(function (usuario) { alteraSelecao(usuario.id) });
        }

        function mudaFiltroStatus(status) {
            atualizaFiltrosDaLocalStorage();
            var indexOf = ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd);
            if (indexOf >= 0) {
                ctrl.filtroStatus.splice(indexOf, 1);
                atualizaFiltrosNaLocalStorage();
                return;
            }
            ctrl.filtroStatus.push(status);
            atualizaFiltrosNaLocalStorage();
        }

        function estaFiltradoPorStatus(status) {
            atualizaFiltrosDaLocalStorage();
            return ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd) >= 0;
        }

        function pegaUsuariosFiltrados() {
            var todosUsuarios = ctrl.listaUsuarios;
            if (ctrl.filtroStatus.length === 0)
                return todosUsuarios;

            return todosUsuarios.filter(function(usuario) { return estaFiltradoPorStatus(usuario.status); });
        }

        function atualizaFiltrosDaLocalStorage() {
            if (localStorageService.isSupported) {
                ctrl.filtroStatus = localStorageService.get(filtroStatusKey) || [ctrl.dominio.statusUsuario.ativo];
            }
        }

        function atualizaFiltrosNaLocalStorage() {
            if (localStorageService.isSupported) {
                localStorageService.set(filtroStatusKey, ctrl.filtroStatus);
            }
        }

        function alteraSelecao(userId) {
            var indexOfUserId = ctrl.usuariosSelecionados.indexOf(userId);
            if (indexOfUserId < 0) {
                ctrl.usuariosSelecionados.push(userId);
                ctrl.usuariosSelecionados[userId] = true;
                return;
            }
            ctrl.usuariosSelecionados.splice(indexOfUserId, 1);
        }

        function confirmaPagamentoSelecionados() {
            processaOperacoesEmLoteParaStatusEspecifico(ctrl.dominio.statusUsuario.aguardandoPagamentoAnuidade, Usuario.confirmaPagamentoSelecionados);
        }

        function cobrancaGeradaSelecionados() {
            processaOperacoesEmLoteParaStatusEspecifico(ctrl.dominio.statusUsuario.novo, Usuario.cobrancaGeradaSelecionados);
        }

        function processaOperacoesEmLoteParaStatusEspecifico(status, metodoNoService) {
            var usuariosSelecionadosComStatus = ctrl.listaUsuarios
                .filter(function (usuario) { return usuario.status.codigoBd === status.codigoBd && ctrl.usuariosSelecionados.indexOf(usuario.id) >= 0; });

            if (usuariosSelecionadosComStatus.length === 0)
                return;

            var idsUsuarios = usuariosSelecionadosComStatus
                .map(function (usuario) { return usuario.id; });

            metodoNoService(idsUsuarios)
                .then(function () { })
                .finally(function() {
                    atualizaListaUsuarios();
                });
        }
    }
})();