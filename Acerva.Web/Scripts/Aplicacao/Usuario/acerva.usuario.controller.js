(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["$scope", "Usuario", "ENUMS", "$uibModal", "DTOptionsBuilder", "localStorageService", UsuarioController]);

    function UsuarioController($scope, Usuario, ENUMS, $uibModal, DTOptionsBuilder, localStorageService) {
        var ctrl = this;
        ctrl.status = {
            carregando: false
        };

        ctrl.listaUsuarios = [];
        ctrl.usuariosSelecionados = [];
        ctrl.todosFiltrados = false;
        ctrl.canceladosCarregados = false;

        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };

        var filtroStatusKey = "usuario.filtroStatus";
        ctrl.filtroStatus = [];

        atualizaFiltrosDaLocalStorage();

        ctrl.selecionaTodosFiltrados = selecionaTodosFiltrados;
        ctrl.buscaCancelados = buscaCancelados;
        ctrl.mudaFiltroStatus = mudaFiltroStatus;
        ctrl.abreJanelaSelecaoEmails = abreJanelaSelecaoEmails;
        ctrl.alteraSelecao = alteraSelecao;
        ctrl.estaFiltradoPorStatus = estaFiltradoPorStatus;
        ctrl.pegaUsuariosFiltrados = pegaUsuariosFiltrados;
        ctrl.confirmaPagamentoSelecionados = confirmaPagamentoSelecionados;
        ctrl.cobrancaGeradaSelecionados = cobrancaGeradaSelecionados;
        ctrl.enviaEmailBoasVindasNaListaSelecionados = enviaEmailBoasVindasNaListaSelecionados;

        ctrl.dtOptions = DTOptionsBuilder.newOptions()
            .withOption('order', [2, 'asc'])
            .withButtons([
                {
                    extend: 'excel',
                    text: 'Excel',
                    footer: true,
                    className: 'btn btn-default btn-sm',
                    name: 'excel'
                }
            ])
        ;

        init();

        function init() {
            Usuario.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;

                return atualizaListaUsuarios();
            });
        }

        function atualizaListaUsuarios(cancelados) {
            if (!cancelados) cancelados = false;
            ctrl.status.carregando = true;
            return Usuario.buscaListaUsuarios(cancelados).then(function (listaUsuarios) {
                ctrl.status.carregando = false;
                ctrl.listaUsuarios = ctrl.listaUsuarios.concat(listaUsuarios);
            });
        }

        function buscaCancelados() {
            if (!ctrl.canceladosCarregados) {
                ctrl.listaUsuarios = ctrl.listaUsuarios.filter(function (u) { return u.status.codigoBd !== ctrl.dominio.statusUsuario.cancelado.codigoBd; });
                ctrl.filtroStatus = ctrl.filtroStatus.filter(function (f) { return f.codigoBd !== ctrl.dominio.statusUsuario.cancelado.codigoBd; });
                atualizaFiltrosNaLocalStorage();
                return;
            }

            return atualizaListaUsuarios(true);
        }

        function selecionaTodosFiltrados() {
            ctrl.usuariosSelecionados = [];
            if (!ctrl.todosFiltrados)
                return;

            ctrl.pegaUsuariosFiltrados().forEach(function (usuario) { alteraSelecao(usuario.id) });
        }

        function abreJanelaSelecaoEmails() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'modal-selecao-emails.html',
                controller: 'SelecaoEmailsModalController',
                controllerAs: 'ctrl',
                resolve: {}
            });

            modalInstance.result.then(selecionaPorEmails);
        }

        function selecionaPorEmails(emails) {
            if (!emails)
                return;

            ctrl.usuariosSelecionados = [];

            var arrEmails = emails
                .replace(/ /g, ",")
                .replace(/\n/g, ",")
                .replace(/\r/g, ",")
                .replace(/\t/g, ",")
                .replace(/;/g, ",")
                .split(",");
            
            ctrl.pegaUsuariosFiltrados()
                .forEach(function (usuario) { if (arrEmails.indexOf(usuario.email) >= 0) alteraSelecao(usuario.id) });
        }

        function mudaFiltroStatus(status) {
            atualizaFiltrosDaLocalStorage();
            var indexOf = ctrl.filtroStatus.map(function (st) { return st.codigoBd }).indexOf(status.codigoBd);

            (indexOf >= 0) ? ctrl.filtroStatus.splice(indexOf, 1) : ctrl.filtroStatus.push(status);

            atualizaFiltrosNaLocalStorage();
        }

        function estaFiltradoPorStatus(status) {
            atualizaFiltrosDaLocalStorage();
            return ctrl.filtroStatus.map(function (st) { return st.codigoBd }).indexOf(status.codigoBd) >= 0;
        }

        function pegaUsuariosFiltrados() {
            var todosUsuarios = ctrl.listaUsuarios;
            if (ctrl.filtroStatus.length === 0)
                return todosUsuarios;

            atualizaFiltrosDaLocalStorage();
            var codigosBdFiltros = ctrl.filtroStatus.map(function(st) { return st.codigoBd });
            return todosUsuarios.filter(function (usuario) { return codigosBdFiltros.indexOf(usuario.status.codigoBd) >= 0; });
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
            processaOperacoesEmLoteParaStatusEspecifico([
                ctrl.dominio.statusUsuario.aguardandoPagamentoAnuidade,
                ctrl.dominio.statusUsuario.aguardandoRenovacao
            ], Usuario.confirmaPagamentoSelecionados);
        }

        function cobrancaGeradaSelecionados() {
            processaOperacoesEmLoteParaStatusEspecifico([
                ctrl.dominio.statusUsuario.novo,
                ctrl.dominio.statusUsuario.ativo
            ], Usuario.cobrancaGeradaSelecionados);
        }

        function enviaEmailBoasVindasNaListaSelecionados() {
            processaOperacoesEmLoteParaStatusEspecifico([
                ctrl.dominio.statusUsuario.ativo
            ], Usuario.enviaEmailBoasVindasNaListaSelecionados);
        }

        function processaOperacoesEmLoteParaStatusEspecifico(arrStatus, metodoNoService) {
            var codigosBdStatus = arrStatus.map(function (status) { return status.codigoBd; });
            var usuariosSelecionadosComStatus = ctrl.listaUsuarios
                .filter(function (usuario) { return codigosBdStatus.indexOf(usuario.status.codigoBd) >= 0 && ctrl.usuariosSelecionados.indexOf(usuario.id) >= 0; });

            if (usuariosSelecionadosComStatus.length === 0)
                return;

            var idsUsuarios = usuariosSelecionadosComStatus
                .map(function (usuario) { return usuario.id; });

            metodoNoService(idsUsuarios)
                .then(function () { })
                .finally(function () {
                    atualizaListaUsuarios();
                });
        }
    }
})();