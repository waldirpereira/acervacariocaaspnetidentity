(function () {
    "use strict";

    angular.module("acerva.meusdados")
        .controller("MeusDadosController", ["$scope", "$timeout", "$routeParams", "$location", "Cropper", "CanalMensagemGrowl", "ENUMS", "MeusDados", MeusDadosController]);

    function MeusDadosController($scope, $timeout, $routeParams, $location, Cropper, CanalMensagemGrowl, ENUMS, MeusDados) {
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

        ctrl.arquivoFoto = null;
        ctrl.dadosFoto = null;

        ctrl.cropperShowEvent = 'show';
        function showCropper() { $scope.$broadcast(ctrl.cropperShowEvent); }
        $scope.onFile = function (blob) {
            Cropper.encode((ctrl.arquivoFoto = blob))
                .then(function (dataUrl) {
                    ctrl.modelo.fotoBase64Url = dataUrl;
                    $timeout(showCropper);  // wait for $digest to set image's src
                });
        };

        ctrl.cropperOptions = {
            movable: true,
            dragMode: "move",
            aspectRatio: 1,
            viewMode: 1,
            autoCropArea: 1,
            minContainerWidth: 100,
            minContainerHeight: 100,
            rotatable: true,
            scalable: true,
            checkOrientation: true,
            crop: function (dataNew) {
                ctrl.dadosFoto = dataNew;
            }
        };

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

            var promise = $timeout();

            if (ctrl.arquivoFoto && ctrl.arquivoFoto.type !== "image/png") {
                ctrl.arquivoFoto = new File([ctrl.arquivoFoto], "newphoto.png", { type: "image/png" });

                promise = Cropper.crop(ctrl.arquivoFoto, ctrl.dadosFoto)
                    .then(function(blob) {
                        var ratio = ctrl.dadosFoto.width > 500 ? 500 / ctrl.dadosFoto.width : 1;
                        return Cropper.scale(blob, ratio);
                    })
                    .then(function(blob) {
                        return Cropper.encode(blob);
                    })
                    .then(function(dataUrl) {
                        return $timeout(function() {
                            ctrl.modelo.fotoBase64 = dataUrl;
                        });
                    });
            }

            promise
                .then(function () {
                    return MeusDados.salvaUsuario(ctrl.modelo);
                })
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