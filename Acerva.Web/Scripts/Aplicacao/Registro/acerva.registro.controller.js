﻿(function () {
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
        ctrl.pegaSrcFoto = pegaSrcFoto;

        var id = $routeParams.id ? $routeParams.id : "";

        init(id);

        function init(id) {
            ctrl.status.carregando = true;

            Registro.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;

                if (id)
                    return;

                colocaUsuarioEmEdicao({ });
                ctrl.status.carregando = false;
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
                .then(function (retorno) {
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

        function pegaSrcFoto() {
            return ctrl.modelo.fotoSelecionada && ctrl.modelo.fotoSelecionada.base64 ? "data:image/png;base64," + ctrl.modelo.fotoSelecionada.base64 : "";
        }
    }

})();