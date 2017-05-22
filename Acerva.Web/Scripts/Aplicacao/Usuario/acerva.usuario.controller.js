(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["$scope", "$timeout", "$q", "Usuario", "ENUMS", "$uibModal", "DTOptionsBuilder", "localStorageService", UsuarioController]);

    function UsuarioController($scope, $timeout, $q, Usuario, ENUMS, $uibModal, DTOptionsBuilder, localStorageService) {
        var ctrl = this;
        ctrl.status = {
            carregando: false
        };

        ctrl.listaUsuarios = [];
        ctrl.usuariosFiltrados = [];
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
        ctrl.confirmaPagamentoSelecionados = confirmaPagamentoSelecionados;
        ctrl.cobrancaGeradaSelecionados = cobrancaGeradaSelecionados;
        ctrl.enviaEmailBoasVindasNaListaSelecionados = enviaEmailBoasVindasNaListaSelecionados;
        ctrl.montaMensagemConfirmacaoOperacaoLote = montaMensagemConfirmacaoOperacaoLote;
        ctrl.getLabelStatusClass = getLabelStatusClass;

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

        function atualizaListaUsuarios(incluiCancelados) {
            if (!incluiCancelados) incluiCancelados = false;
            ctrl.status.carregando = true;
            return Usuario.buscaListaUsuarios(incluiCancelados).then(function (listaUsuarios) {
                ctrl.status.carregando = false;
                ctrl.listaUsuarios = listaUsuarios;
                atualizaUsuariosFiltrados();
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

            ctrl.usuariosFiltrados.forEach(function (usuario) { alteraSelecao(usuario.id) });
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

        function selecionaPorEmails(selecionaPorEmailsModel) {
            if (!selecionaPorEmailsModel || !selecionaPorEmailsModel.emails)
                return;

            var arrEmails = selecionaPorEmailsModel.emails
                .replace(/ /g, ",")
                .replace(/\n/g, ",")
                .replace(/\r/g, ",")
                .replace(/\t/g, ",")
                .replace(/;/g, ",")
                .split(",");

            if (selecionaPorEmailsModel.limpaSelecaoAtual)
                ctrl.usuariosSelecionados = [];

            ctrl.usuariosFiltrados
                .filter(function (usuario) {
                    return arrEmails.indexOf(usuario.email) >= 0;
                })
                .map(function (usuario) {
                    return usuario.id;
                })
                .map(alteraSelecao);
        }

        function mudaFiltroStatus(status) {
            atualizaFiltrosDaLocalStorage();
            var indexOf = ctrl.filtroStatus.map(function (st) { return st.codigoBd }).indexOf(status.codigoBd);

            (indexOf >= 0) ? ctrl.filtroStatus.splice(indexOf, 1) : ctrl.filtroStatus.push(status);

            atualizaFiltrosNaLocalStorage();
            atualizaUsuariosFiltrados();
        }

        function estaFiltradoPorStatus(status) {
            atualizaFiltrosDaLocalStorage();
            return ctrl.filtroStatus.map(function (st) { return st.codigoBd }).indexOf(status.codigoBd) >= 0;
        }

        function atualizaUsuariosFiltrados() {
            var todosUsuarios = ctrl.listaUsuarios;

            atualizaFiltrosDaLocalStorage();
            var codigosBdFiltros = ctrl.filtroStatus.map(function (st) { return st.codigoBd });

            ctrl.status.carregando = true;
            $timeout(function () {
                if (ctrl.filtroStatus.length === 0)
                    ctrl.usuariosFiltrados = todosUsuarios;
                else
                    ctrl.usuariosFiltrados = todosUsuarios.filter(function (usuario) { return codigosBdFiltros.indexOf(usuario.status.codigoBd) >= 0; });
                ctrl.status.carregando = false;
            }, 0);
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

        function getLabelStatusClass(status) {
            var cls;
            switch(status.codigoBd) {
                case ctrl.dominio.statusUsuario.ativo.codigoBd:
                    cls = "label-success";
                    break;
                case ctrl.dominio.statusUsuario.cancelado.codigoBd:
                    cls = "label-danger";
                    break;
                case ctrl.dominio.statusUsuario.aguardandoIndicacao.codigoBd:
                    cls = "label-warning";
                    break;
                case ctrl.dominio.statusUsuario.aguardandoConfirmacaoEmail.codigoBd:
                case ctrl.dominio.statusUsuario.novo.codigoBd:
                    cls = "label-default";
                    break;
                case ctrl.dominio.statusUsuario.aguardandoPagamentoAnuidade.codigoBd:
                case ctrl.dominio.statusUsuario.aguardandoRenovacao.codigoBd:
                    cls = "label-primary";
            }
            return cls;
        }

        function montaMensagemConfirmacaoOperacaoLote(pergunta) {
            var usuariosSelecionados = ctrl.usuariosFiltrados
                .filter(function (usuario) {
                    return ctrl.usuariosSelecionados.indexOf(usuario.id) >= 0;
                })
                .map(function (usuario) {
                    return {
                        name: usuario.name,
                        status: usuario.status
                    };
                });

            var listaUsuarios = "";
            if (usuariosSelecionados.length) {

                listaUsuarios = "<br/><br/><ul><li>"
                    + usuariosSelecionados
                    .map(function (usuario) {
                        return usuario.name + " <span class='label " + getLabelStatusClass(usuario.status) + "'>" + usuario.status.nomeExibicao + "</span>";
                    })
                    .join("</li><li>")
                    + "</li></ul>";
            }

            pergunta = pergunta.replace("%QUANTIDADE%", usuariosSelecionados.length > 0 ? usuariosSelecionados.length : "");

            return pergunta + listaUsuarios;
        }
    }
})();