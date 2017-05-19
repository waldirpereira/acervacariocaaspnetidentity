(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("CadastroUsuarioController", ["$scope", "$timeout", "$routeParams", "$location", "$uibModal", "ENUMS", "Cropper", "CanalMensagemGrowl", "Usuario", CadastroUsuarioController]);

    function CadastroUsuarioController($scope, $timeout, $routeParams, $location, $uibModal, ENUMS, Cropper, CanalMensagemGrowl, Usuario) {
        var ctrl = this;
        ctrl.status = {
            carregando: false,
            salvando: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};

        ctrl.tipoCadastro = "usuario";

        ctrl.pegaSrcFoto = pegaSrcFoto;
        ctrl.salvaUsuario = salvaUsuario;
        ctrl.recuperaUsuariosIndicacao = recuperaUsuariosIndicacao;
        ctrl.confirmaPagamento = confirmaPagamento;
        ctrl.cobrancaGerada = cobrancaGerada;
        ctrl.confirmaEmail = confirmaEmail;
        ctrl.confirmaIndicacao = confirmaIndicacao;
        ctrl.recusaIndicacao = recusaIndicacao;
        ctrl.cancelaUsuario = cancelaUsuario;
        ctrl.reativaUsuario = reativaUsuario;
        ctrl.voltarParaAguardandoConfirmacaoEmail = voltarParaAguardandoConfirmacaoEmail;
        ctrl.voltarParaAguardandoIndicacao = voltarParaAguardandoIndicacao;
        ctrl.voltaParaNovo = voltaParaNovo;
        ctrl.voltaParaAtivo = voltaParaAtivo;
        ctrl.mostraHistoricoStatus = mostraHistoricoStatus;

        ctrl.arquivoFoto = null;
        ctrl.dadosFoto = null;

        ctrl.cropperShowEvent = 'show';
        function showCropper() { $scope.$broadcast(ctrl.cropperShowEvent); }
        $scope.onFile = function (blob) {
            Cropper.encode((ctrl.arquivoFoto = blob)).then(function (dataUrl) {
                ctrl.modelo.fotoBase64Url = dataUrl;
                $timeout(showCropper);  // wait for $digest to set image's src
            });
        };

        ctrl.cropperOptions = {
            movable: true,
            dragMode: "move",
            aspectRatio: 1,
            viewMode: 2,
            autoCropArea: 1,
            minContainerWidth: 200,
            minContainerHeight: 200,
            rotatable: true,
            scalable: true,
            checkOrientation: true,
            crop: function (dataNew) {
                ctrl.dadosFoto = dataNew;
            }
        };

        ctrl.formularioDesabilitado = function() {
            return ctrl.modeloOriginal.id && !ctrl.dominio.usuarioLogadoEhAdmin && !ctrl.dominio.usuarioLogadoEhDiretor && ctrl.dominio.idUsuarioLogado !== ctrl.modeloOriginal.id;
        }

        var id = $routeParams.id ? $routeParams.id : "";
        init(id);

        function init(id) {
            ctrl.status.carregando = true;

            Usuario.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                ctrl.dominio.statusUsuario = ENUMS.statusUsuario;
                ctrl.dominio.listaStatusUsuario = ENUMS.toArrayOfEnums(ENUMS.statusUsuario);
                ctrl.dominio.sexo = ENUMS.sexo;
                ctrl.dominio.sexos = ENUMS.toArrayOfEnums(ENUMS.sexo);

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

        function pegaSrcFoto() {
            return ctrl.modelo.fotoSelecionada && ctrl.modelo.fotoSelecionada.base64 ? "data:image/png;base64," + ctrl.modelo.fotoSelecionada.base64 : "";
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

            if (ctrl.arquivoFoto && ctrl.arquivoFoto.type !== "image/png")
                ctrl.arquivoFoto = new File([ctrl.arquivoFoto], "newphoto.png", { type: "image/png" });

            Cropper.crop(ctrl.arquivoFoto, ctrl.dadosFoto)
                .then(function (blob) {
                    var ratio = ctrl.dadosFoto.width > 500 ? 500 / ctrl.dadosFoto.width : 1;
                    return Cropper.scale(blob, ratio);
                })
                .then(Cropper.encode)
                .then(function (dataUrl) {
                    return $timeout(function () {
                        ctrl.modelo.fotoBase64 = dataUrl;
                    });
                })
                .then(function () {
                    return Usuario.salvaUsuario(ctrl.modelo);
                })
                .then(function (retorno) {
                    
                })
                .finally(function() {
                    ctrl.status.salvando = false;
                    $location.path("/");
                });
        }

        function recuperaUsuariosIndicacao(termo) {
            return Usuario.buscaUsuariosAtivosComTermo(termo).then(function (usuarios) {
                return usuarios;
            });
        }

        function executaAcaoComUsuario(usuario, metodoNoService) {
            metodoNoService(usuario)
                .then(function () { })
                .finally(function() {
                    init(usuario.id);
                });
        }

        function confirmaEmail(usuario) {
            executaAcaoComUsuario(usuario, Usuario.confirmaEmail);
        }

        function confirmaPagamento(usuario) {
            executaAcaoComUsuario(usuario, Usuario.confirmaPagamento);
        }

        function cobrancaGerada(usuario) {
            executaAcaoComUsuario(usuario, Usuario.cobrancaGerada);
        }

        function confirmaIndicacao(usuario) {
            executaAcaoComUsuario(usuario, Usuario.confirmaIndicacao);
        }

        function recusaIndicacao(usuario) {
            executaAcaoComUsuario(usuario, Usuario.recusaIndicacao);
        }

        function cancelaUsuario(usuario) {
            executaAcaoComUsuario(usuario, Usuario.cancelaUsuario);
        }

        function reativaUsuario(usuario) {
            executaAcaoComUsuario(usuario, Usuario.reativaUsuario);
        }

        function voltarParaAguardandoConfirmacaoEmail(usuario) {
            executaAcaoComUsuario(usuario, Usuario.voltarParaAguardandoConfirmacaoEmail);
        }

        function voltarParaAguardandoIndicacao(usuario) {
            executaAcaoComUsuario(usuario, Usuario.voltarParaAguardandoIndicacao);
        }

        function voltaParaNovo(usuario) {
            executaAcaoComUsuario(usuario, Usuario.voltaParaNovo);
        }

        function voltaParaAtivo(usuario) {
            executaAcaoComUsuario(usuario, Usuario.voltaParaAtivo);
        }

        function mostraHistoricoStatus(usuario) {
            Usuario.buscaHistoricoStatus(usuario.id).then(function (listaHistorico) {
                $uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'modal-historico-status.html',
                    controller: 'HistoricoStatusModalController',
                    controllerAs: 'ctrl',
                    resolve: {
                        usuario: usuario,
                        dominio: {statusUsuario:ctrl.dominio.statusUsuario},
                        listaHistorico: function () {
                            return listaHistorico;
                        }
                    }
                });
            });
        }
    }
})();