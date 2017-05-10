﻿(function () {
    "use strict";

    angular.module("acerva.meusdados")
        .controller("MeusDadosController", ["$scope", "$timeout", "$routeParams", "$location", "CanalMensagemGrowl", "ENUMS", "MeusDados", MeusDadosController]);

    function MeusDadosController($scope, $timeout, $routeParams, $location, CanalMensagemGrowl, ENUMS, MeusDados) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.tipoCadastro = "meusdados";

        ctrl.salvaUsuario = salvaUsuario;
        ctrl.recuperaUsuariosIndicacao = recuperaUsuariosIndicacao;
        ctrl.pegaSrcFoto = pegaSrcFoto;

        ctrl.formularioDesabilitado = function() {
            return ctrl.dominio.idUsuarioLogado !== ctrl.modeloOriginal.id;
        }

        init();

        function init() {
            ctrl.status.carregando = true;

            MeusDados.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;
                ctrl.dominio.sexo = ENUMS.sexo;
                ctrl.dominio.sexos = ENUMS.toArrayOfEnums(ENUMS.sexo);

                return MeusDados.buscaUsuarioLogadoParaEdicao().then(function (usuario) {
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
            if ($scope.formUsuario.$invalid) {
                var mensagemGrowl = {
                    message: "Por favor verifique os campos marcados em vermelho.",
                    severity: "error",
                    config: {
                        title: "Não foi possível salvar"
                    }
                };
                CanalMensagemGrowl.enviaNovaMensagem(mensagemGrowl);
                return;
            }
            ctrl.status.salvando = true;

            MeusDados.salvaUsuario(ctrl.modelo)
                .then(function (retorno) {
                    if (retorno === "OK") {
                        $location.path("/Edited");
                        return;
                    }
                })
                .finally(function () { ctrl.status.salvando = false; });
        }

        function recuperaUsuariosIndicacao(termo) {
            return MeusDados.buscaUsuariosAtivosComTermo(termo).then(function (usuarios) {
                return usuarios;
            });
        }

        function pegaSrcFoto() {
            return ctrl.modelo.fotoSelecionada && ctrl.modelo.fotoSelecionada.base64 ? "data:image/png;base64," + ctrl.modelo.fotoSelecionada.base64 : "";
        }
    }

})();