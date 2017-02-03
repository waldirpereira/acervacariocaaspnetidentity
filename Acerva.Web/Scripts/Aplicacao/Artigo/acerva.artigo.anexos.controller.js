(function () {
    "use strict";

    angular.module("acerva.artigo")
        .controller("ArtigoAnexosModalController", ["$scope", "$uibModalInstance", "Artigo", "artigo", "anexos", ArtigoAnexosModalController]);

    function ArtigoAnexosModalController($scope, $uibModalInstance, Artigo, artigo, anexos) {
        var ctrl = this;

        ctrl.modelo = {};

        ctrl.artigo = artigo;
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

            Artigo.salvaAnexo(ctrl.artigo.codigo, ctrl.modelo.arquivo, ctrl.modelo.titulo)
                .then(function () {
                    ctrl.modelo = {};
                    return Artigo.buscaAnexos(ctrl.artigo.codigo);
                })
            .then(function (anexos) {
                ctrl.anexos = anexos;
            });
        }

        function excluiAnexo(anexo) {
            Artigo.excluiAnexo(anexo.codigo)
                .then(function () {
                    return Artigo.buscaAnexos(ctrl.artigo.codigo);
                })
            .then(function (anexos) {
                ctrl.anexos = anexos;
            });
        }
    }

})();