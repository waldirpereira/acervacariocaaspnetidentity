(function () {
    "use strict";

    angular.module("acerva.registro")
        .controller("RegistroController", ["$scope", "$timeout", "$routeParams", "$location", "ENUMS", "Registro", RegistroController]);

    function RegistroController($scope, $timeout, $routeParams, $location, ENUMS, Registro) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.tipoCadastro = "registro";

        ctrl.salvaUsuario = salvaUsuario;
        ctrl.recuperaUsuariosIndicacao = recuperaUsuariosIndicacao;

        var id = $routeParams.id ? $routeParams.id : "";
        init(id);

        function init(id) {
            ctrl.status.carregando = true;

            Registro.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;

                if (!id) {
                    colocaUsuarioEmEdicao({ status: ctrl.dominio.statusUsuario.cancelado });
                    ctrl.status.carregando = false;
                    return;
                }

                return Registro.buscaUsuario(id).then(function (usuario) {
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
            
            Registro.salvaUsuario(ctrl.modelo)
                .then(function(retorno) {
                    if (retorno === "OK") {
                        $location.path("/ConfirmSent");
                        return;
                    }
                })
                .finally(function () { ctrl.status.salvando = false; });
        }

        function recuperaUsuariosIndicacao(termo) {
            return Registro.buscaUsuariosAtivosComTermo(termo).then(function (usuarios) {
                return usuarios;
            });
        }
    }

})();