(function () {
    "use strict";

    angular.module("acerva.situacao")
        .controller("SituacaoController", ["Situacao", SituacaoController]);

    function SituacaoController(Situacao) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.termo = null;
        ctrl.modelo = null;

        ctrl.buscaSituacao = buscaSituacao;

        init();

        function init() {
            
        }

        function buscaSituacao(termo) {
            ctrl.status.carregando = true;
            return Situacao.buscaSituacao(termo).then(function (usuario) {
                ctrl.modelo = usuario;
                ctrl.status.carregando = false;
            });
        }
    }
})();