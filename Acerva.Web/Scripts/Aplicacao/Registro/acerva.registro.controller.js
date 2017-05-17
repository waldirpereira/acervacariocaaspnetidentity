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

        var file, data;

        /**
         * Method is called every time file input's value changes.
         * Because of Angular has not ng-change for file inputs a hack is needed -
         * call `angular.element(this).scope().onFile(this.files[0])`
         * when input's event is fired.
         */
        $scope.onFile = function (blob) {
            Cropper.encode((file = blob)).then(function (dataUrl) {
                $scope.dataUrl = dataUrl;
                $timeout(showCropper);  // wait for $digest to set image's src
            });
        };

        /**
         * Croppers container object should be created in controller's scope
         * for updates by directive via prototypal inheritance.
         * Pass a full proxy name to the `ng-cropper-proxy` directive attribute to
         * enable proxing.
         */
        $scope.cropper = {};
        $scope.cropperProxy = 'cropper.first';

        /**
         * When there is a cropped image to show encode it to base64 string and
         * use as a source for an image element.
         */
        $scope.preview = function () {
            if (!file || !data) return;
            Cropper.crop(file, data).then(Cropper.encode).then(function (dataUrl) {
                ($scope.preview || ($scope.preview = {})).dataUrl = dataUrl;
            });
        };

        /**
         * Use cropper function proxy to call methods of the plugin.
         * See https://github.com/fengyuanchen/cropper#methods
         */
        $scope.clear = function (degrees) {
            if (!$scope.cropper.first) return;
            $scope.cropper.first('clear');
        };

        /**
         * Object is used to pass options to initalize a cropper.
         * More on options - https://github.com/fengyuanchen/cropper#options
         */
        $scope.options = {
            movable: true,
            dragMode: "move",
            aspectRatio: 1,
            viewMode: 2,
            autoCropArea: 1,
            minContainerWidth: 200,
            minContainerHeight: 200,
            crop: function (dataNew) {
                data = dataNew;
            }
        };

        /**
         * Showing (initializing) and hiding (destroying) of a cropper are started by
         * events. The scope of the `ng-cropper` directive is derived from the scope of
         * the controller. When initializing the `ng-cropper` directive adds two handlers
         * listening to events passed by `ng-cropper-show` & `ng-cropper-hide` attributes.
         * To show or hide a cropper `$broadcast` a proper event.
         */
        $scope.showEvent = 'show';
        $scope.hideEvent = 'hide';

        function showCropper() { $scope.$broadcast($scope.showEvent); }

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

            Cropper.crop(file, data)
                .then(function (blob) {
                    var ratio = 200 / data.width;
                    return Cropper.scale(blob, ratio);
                })
                .then(Cropper.encode)
                .then(function (dataUrl) {
                    return $timeout(function () {
                        ctrl.modelo.fotoBase64 = dataUrl.replace("data:image/jpeg;base64,", "");
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