(function () {
    "use strict";

    angular.module("acerva.meusdados")
        .controller("MeusDadosController", ["$scope", "$timeout", "$routeParams", "$location", "$uibModal", "CanalMensagemGrowl", "ENUMS", "MeusDados", MeusDadosController]);

    function MeusDadosController($scope, $timeout, $routeParams, $location, $uibModal, CanalMensagemGrowl, ENUMS, MeusDados) {
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
        ctrl.abrirModalTrocaFoto = abrirModalTrocaFoto;
        

        ctrl.formularioDesabilitado = function () {
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

        function abrirModalTrocaFoto() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'modal-troca-foto.html',
                controller: 'TrocaFotoModalController',
                controllerAs: 'ctrl',
                backdrop: "static",
                resolve: { }
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