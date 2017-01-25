(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("CadastroUsuarioController", ["$scope", "$timeout", "$routeParams", "$location", "ENUMS", "CanalMensagemGrowl", "Usuario", CadastroUsuarioController]);

    function CadastroUsuarioController($scope, $timeout, $routeParams, $location, ENUMS, CanalMensagemGrowl, Usuario) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.salvaUsuario = salvaUsuario;
        ctrl.recuperaUsuariosIndicacao = recuperaUsuariosIndicacao;
        ctrl.confirmaPagamento = confirmaPagamento;
        ctrl.cobrancaGerada = cobrancaGerada;
        ctrl.confirmaEmail = confirmaEmail;
        ctrl.confirmaIndicacao = confirmaIndicacao;
        ctrl.recusaIndicacao = recusaIndicacao;
        ctrl.cancelaUsuario = cancelaUsuario;
        ctrl.reativaUsuario = reativaUsuario;

        var id = $routeParams.id ? $routeParams.id : "";
        init(id);

        function init(id) {
            ctrl.status.carregando = true;

            Usuario.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;
                ctrl.dominio.listaStatusUsuario = ENUMS.toArrayOfEnums(ENUMS.statusUsuario);

                if (!id) {
                    colocaUsuarioEmEdicao({ status: ctrl.dominio.statusUsuario.novo });
                    ctrl.status.carregando = false;
                    return;
                }

                return Usuario.buscaUsuario(id).then(function (usuario) {
                    colocaUsuarioEmEdicao(usuario);
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaUsuarioEmEdicao(usuario) {
            ctrl.modeloOriginal = usuario;
            ctrl.modelo = angular.copy(usuario);
        }

        function salvaUsuario() {
            if ($scope.formUsuario.$invalid)
                return;

            ctrl.status.salvando = true;
            Usuario.salvaUsuario(ctrl.modelo)
                .then(function () {  })
                .finally(function() {
                    $location.path("/");
                });
        }

        function recuperaUsuariosIndicacao(termo) {
            return Usuario.buscaUsuariosAtivosComTermo(termo).then(function (usuarios) {
                return usuarios;
            });
        }

        function confirmaEmail(usuario) {
            Usuario.confirmaEmail(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function confirmaPagamento(usuario) {
            Usuario.confirmaPagamento(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function cobrancaGerada(usuario) {
            Usuario.cobrancaGerada(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function confirmaIndicacao(usuario) {
            Usuario.confirmaIndicacao(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function recusaIndicacao(usuario) {
            Usuario.recusaIndicacao(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function cancelaUsuario(usuario) {
            Usuario.cancelaUsuario(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }

        function reativaUsuario(usuario) {
            Usuario.reativaUsuario(usuario)
                .then(function () { })
                .finally(function() {
                    $location.path("/");
                });
        }
    }

})();