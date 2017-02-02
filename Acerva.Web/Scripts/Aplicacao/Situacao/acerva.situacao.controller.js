(function () {
    "use strict";

    angular.module("acerva.situacao")
        .controller("SituacaoController", ["ENUMS", "Situacao", SituacaoController]);

    function SituacaoController(ENUMS, Situacao) {
        var ctrl = this;

        ctrl.status = {
            carregando: false
        };

        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };

        ctrl.termo = null;
        ctrl.modelo = null;

        ctrl.buscaSituacao = buscaSituacao;
        
        init();

        function init() { }

        function buscaSituacao(cpf) {
            if (!cpf)
                return;

            ctrl.status.carregando = true;
            return Situacao.buscaSituacao(cpf).then(function (usuario) {
                ctrl.modelo = usuario;
                ctrl.status.carregando = false;
            });
        }
    }
})();