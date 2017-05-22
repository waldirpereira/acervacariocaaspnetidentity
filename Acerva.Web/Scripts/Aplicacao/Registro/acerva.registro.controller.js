(function () {
    "use strict";

    angular.module("acerva.registro")
        .controller("RegistroController", ["$scope", "$timeout", "$routeParams", "$location", "$uibModal", "ENUMS", "CanalMensagemGrowl", "Registro", RegistroController]);

    function RegistroController($scope, $timeout, $routeParams, $location, $uibModal, ENUMS, CanalMensagemGrowl, Registro) {
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
        ctrl.abrirModalTrocaFoto = abrirModalTrocaFoto;

        ctrl.formularioDesabilitado = function () {
            return ctrl.modeloOriginal.id && !ctrl.dominio.usuarioLogadoEhAdmin && !ctrl.dominio.usuarioLogadoEhDiretor && ctrl.dominio.idUsuarioLogado !== ctrl.modeloOriginal.id;
        }

        var id = $routeParams.id ? $routeParams.id : "";

        init(id);

        function init(id) {
            ctrl.status.carregando = true;

            Registro.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;
                ctrl.dominio.sexo = ENUMS.sexo;
                ctrl.dominio.sexos = ENUMS.toArrayOfEnums(ENUMS.sexo);

                if (id)
                    return;

                colocaUsuarioEmEdicao({});
                ctrl.status.carregando = false;
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

        function abrirModalTrocaFoto() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'modal-troca-foto.html',
                controller: 'TrocaFotoModalController',
                controllerAs: 'ctrl',
                backdrop: "static",
                resolve: {}
            });

            modalInstance.result.then(trocaFoto);
        }

        function trocaFoto(fotoBase64) {
            if (!fotoBase64)
                return;

            ctrl.modelo.fotoBase64 = fotoBase64;
        }
    }

})();