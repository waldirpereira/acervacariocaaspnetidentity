(function () {
    "use strict";

    angular.module("acerva.registro")
        .controller("RegistroController", ["$scope", "$timeout", "$routeParams", "$location", "ENUMS", "Cropper", "CanalMensagemGrowl", "Registro", RegistroController]);

    function RegistroController($scope, $timeout, $routeParams, $location, ENUMS, Cropper, CanalMensagemGrowl, Registro) {
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
                    return Registro.salvaUsuario(ctrl.modelo);
                })
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

        function pegaSrcFoto() {dragMode
            return ctrl.modelo.fotoSelecionada && ctrl.modelo.fotoSelecionada.base64 ? "data:image/png;base64," + ctrl.modelo.fotoSelecionada.base64 : "";
        }
    }

})();