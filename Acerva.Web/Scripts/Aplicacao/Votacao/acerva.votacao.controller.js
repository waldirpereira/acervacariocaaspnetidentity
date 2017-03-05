(function () {
    "use strict";

    angular.module("acerva.votacao")
        .controller("VotacaoController", ["Votacao", VotacaoController]);

    function VotacaoController(Votacao) {
        var ctrl = this;

        ctrl.listaVotacoes = [];

        ctrl.ativa = ativa;
        ctrl.inativa = inativa;

        init();

        function init() {
            atualizaListaVotacoes();
        }

        function atualizaListaVotacoes() {
            Votacao.buscaListaVotacoes().then(function (listaVotacoes) {
                ctrl.listaVotacoes = listaVotacoes;
            });
        }

        function ativa(votacao) {
            Votacao.ativa(votacao).then(function () {
                votacao.ativo = true;
            });
        }

        function inativa(votacao) {
            Votacao.inativa(votacao).then(function () {
                votacao.ativo = false;
            });
        }
    }
})();