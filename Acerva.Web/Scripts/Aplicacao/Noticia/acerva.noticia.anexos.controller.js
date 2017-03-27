(function () {
    "use strict";

    angular.module("acerva.noticia")
        .controller("NoticiaAnexosModalController", ["$scope", "$uibModalInstance", "Noticia", "noticia", "anexos", NoticiaAnexosModalController]);

    function NoticiaAnexosModalController($scope, $uibModalInstance, Noticia, noticia, anexos) {
        var ctrl = this;

        ctrl.modelo = {};

        ctrl.noticia = noticia;
        ctrl.anexos = anexos;
        ctrl.isFormSubmitted = null;

        $scope.selecionaArquivoParaUpload = function (file) {
            ctrl.modelo.arquivo = file[0];
        }
        ctrl.ok = ok;
        ctrl.anexaArquivo = anexaArquivo;
        ctrl.excluiAnexo = excluiAnexo;

        function ok() {
            $uibModalInstance.close();
        }

        function anexaArquivo() {
            ctrl.isFormSubmitted = true;

            Noticia.salvaAnexo(ctrl.noticia.codigo, ctrl.modelo.arquivo, ctrl.modelo.titulo)
                .then(function () {
                    ctrl.modelo = {};
                    return Noticia.buscaAnexos(ctrl.noticia.codigo);
                })
            .then(function (anexos) {
                ctrl.anexos = anexos;
            });
        }

        function excluiAnexo(anexo) {
            Noticia.excluiAnexo(anexo.codigo)
                .then(function () {
                    return Noticia.buscaAnexos(ctrl.noticia.codigo);
                })
            .then(function (anexos) {
                ctrl.anexos = anexos;
            });
        }
    }

})();