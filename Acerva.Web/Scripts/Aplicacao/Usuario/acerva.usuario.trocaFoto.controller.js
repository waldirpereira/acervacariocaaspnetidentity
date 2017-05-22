(function () {
    "use strict";

    angular.module("acerva")
        .controller("TrocaFotoModalController", ["$scope", "$uibModalInstance", "$timeout", "Cropper", TrocaFotoModalController]);

    function TrocaFotoModalController($scope, $uibModalInstance, $timeout, Cropper) {
        var ctrl = this;

        ctrl.modelo = {};

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

        ctrl.ok = ok;

        function ok() {
            if (ctrl.arquivoFoto) {

                if (ctrl.arquivoFoto.type !== "image/png")
                    ctrl.arquivoFoto = new File([ctrl.arquivoFoto], "newphoto.png", { type: "image/png" });

                Cropper.crop(ctrl.arquivoFoto, ctrl.dadosFoto)
                    .then(function (blob) {
                        var ratio = ctrl.dadosFoto.width > 500 ? 500 / ctrl.dadosFoto.width : 1;
                        return Cropper.scale(blob, ratio);
                    })
                    .then(function (blob) {
                        return Cropper.encode(blob);
                    })
                    .then(function (dataUrl) {
                        return $timeout(function () {
                            $uibModalInstance.close(dataUrl);
                        });
                    });
            }
        }
    }

})();