(function () {
    "use strict";

    angular.module("acerva.minhaAcerva")
        .controller("CadastroMinhaAcervaController", ["$scope", "$routeParams", "$location", "MinhaAcerva", CadastroMinhaAcervaController]);

    function CadastroMinhaAcervaController($scope, $routeParams, $location, MinhaAcerva) {
        var ctrl = this;

        ctrl.status = {
            carregando: false,
            salvando: false,
            bloqueado: false
        };

        ctrl.modelo = {};
        ctrl.modeloOriginal = {};
        ctrl.dominio = {};
        ctrl.graficoEvolucao = {
            data: [],
            series: [],
            labels: [],
            datasetOverride: [],
            options: {},
            participacoes: []
        };

        ctrl.participacaoEmEdicao = null;
        ctrl.participacaoEmEdicaoOriginal = null;

        ctrl.salvaMinhaAcerva = salvaMinhaAcerva;
        ctrl.buscaCodigoParticipacao = buscaCodigoParticipacao;

        ctrl.incluiParticipacao = incluiParticipacao;
        ctrl.salvaParticipacao = salvaParticipacao;
        ctrl.editaParticipacao = editaParticipacao;
        ctrl.excluiParticipacao = excluiParticipacao;
        ctrl.cancelaEdicaoParticipacao = cancelaEdicaoParticipacao;
        ctrl.reenviaConviteParticipacao = reenviaConviteParticipacao;

        ctrl.incrementa = incrementa;
        ctrl.decrementa = decrementa;

        var idMinhaAcerva = $routeParams.id ? parseInt($routeParams.id) : 0;
        init(idMinhaAcerva);

        function init(id) {
            ctrl.status.carregando = true;

            MinhaAcerva.buscaTiposDominio().then(function (tipos) {
                angular.extend(ctrl.dominio, tipos);

                if (id === 0) {
                    colocaMinhaAcervaEmEdicao({ ativo: true, usuarioResponsavel: AcervaApp.user, regras: ctrl.dominio.novasRegras || [] });
                    incluiParticipacao();
                    salvaParticipacao();
                    ctrl.status.carregando = false;
                    return;
                }

                return MinhaAcerva.buscaMinhaAcerva(id).then(function (minhaAcerva) {
                    colocaMinhaAcervaEmEdicao(minhaAcerva);
                    MinhaAcerva.buscaEvolucao(id).then(function (evolucao) {
                        if (!evolucao)
                            return;

                        ctrl.evolucao = evolucao;
                        ctrl.graficoEvolucao.labels = ctrl.evolucao.rodadas;
                        ctrl.evolucao.registrosEvolucao = ctrl.evolucao.registrosEvolucao;
                        configuraGraficoEvolucao();
                    });
                    ctrl.status.carregando = false;
                });
            });
        }

        function colocaMinhaAcervaEmEdicao(minhaAcerva) {
            ctrl.status.bloqueado = minhaAcerva.codigo && (!minhaAcerva.ativo || minhaAcerva.usuarioResponsavel.id !== AcervaApp.user.id);
            ctrl.modeloOriginal = minhaAcerva;
            ctrl.modelo = angular.copy(minhaAcerva);
        }

        function salvaMinhaAcerva() {
            if ($scope.formMinhaAcerva.$invalid)
                return;

            ctrl.status.salvando = true;

            MinhaAcerva.salvaMinhaAcerva(ctrl.modelo)
                .then(function () { $location.path("/"); })
                .finally(function () { ctrl.status.salvando = false; });
        }

        function buscaCodigoParticipacao() {
            if (!ctrl.modelo.participacoes || !AcervaApp.user)
                return null;

            var participacao = _.filter(ctrl.modelo.participacoes, function(p) { return p.usuario && p.usuario.id === AcervaApp.user.id })[0] || {codigo: 0};
            return participacao.codigo;
        }

        // PARTICIPACAO - INICIO
        function incrementa(valor) {
            valor = valor !== null ? +valor : null;
            if (!_.isFinite(valor)) {
                return 0;
            }
            return ++valor;
        }

        function decrementa(valor) {
            valor = +valor;
            if (!_.isFinite(valor) || valor === 0) {
                return 0;
            }
            return --valor;
        }

        function incluiParticipacao() {
            if (!ctrl.modelo.participacoes)
                ctrl.modelo.participacoes = [];

            var minCodigo = (_.minBy(ctrl.modelo.participacoes, "codigo") || {}).codigo;
            minCodigo = minCodigo < 0 ? minCodigo : 0;
            var nova = { codigo: minCodigo - 1, pontuacaoInicial: 0 };
            if (ctrl.modelo.participacoes.length === 0) {
                nova.usuario = AcervaApp.user;
                nova.pontuacaoInicial = 0;
            }

            ctrl.participacaoEmEdicao = nova;
            ctrl.modelo.participacoes.push(nova);
        }

        function editaParticipacao(participacao) {
            if (ctrl.participacaoEmEdicao)
                cancelaEdicaoParticipacao();
            ctrl.participacaoEmEdicaoOriginal = angular.copy(participacao);
            ctrl.participacaoEmEdicao = participacao;
        }

        function excluiParticipacao(participacao) {
            var indiceParticipacaoExcluida = _.findIndex(ctrl.modelo.participacoes, function (f) {
                return f.codigo === participacao.codigo;
            });

            _.pullAt(ctrl.modelo.participacoes, indiceParticipacaoExcluida);
        }

        function reenviaConviteParticipacao(participacao) {
            MinhaAcerva.reenviaConviteParticipacao(participacao)
                .finally(function () { ctrl.status.salvando = false; });
        }

        function cancelaEdicaoParticipacao() {
            if (!ctrl.participacaoEmEdicaoOriginal) {
                _.remove(ctrl.modelo.participacoes, function (f) {
                    return f.codigo === ctrl.participacaoEmEdicao.codigo;
                });
            } else {
                var indiceParticipacaoEditada = _.findIndex(ctrl.modelo.participacoes, function (f) {
                    return f.codigo === ctrl.participacaoEmEdicaoOriginal.codigo;
                });
                ctrl.modelo.participacoes[indiceParticipacaoEditada] = angular.copy(ctrl.participacaoEmEdicaoOriginal);
            }

            ctrl.participacaoEmEdicao = null;
            ctrl.participacaoEmEdicaoOriginal = null;
        }

        function salvaParticipacao() {
            ctrl.participacaoEmEdicao = null;
            ctrl.participacaoEmEdicaoOriginal = null;
        }
        // PARTICIPACAO - FIM

        // EVOLUCAO - INICIO
        function montaEixoParticipacao(codigoParticipacao) {
            return {
                id: 'y-axis-' + codigoParticipacao,
                type: 'linear',
                display: false,
                gridLines: {
                    display: false
                },
                ticks: {
                    reverse: true,
                    max: ctrl.evolucao.registrosEvolucao.length,
                    min: 1,
                    stepSize: 1
                }
            };
        }
        function montaOverrideParticipacao(codigoParticipacao) {
            return {
                yAxisID: 'y-axis-' + codigoParticipacao,
                lineTension: 0,
                fill: false
            };
        }
        function pegaParticipanteNaPosicaoNaUltimaRodada(posicao) {
            var participante;
            _(ctrl.evolucao.registrosEvolucao).forEach(function (participacao) {
                if (_(participacao.posicoes).filter(function (p) { return !!p; }).last() === posicao) {
                    participante = participacao.nome;
                    return;
                }
            });
            return participante;
        }
        
        function configuraGraficoEvolucao() {
            ctrl.graficoEvolucao.options = {
                responsive: true,
                maintainAspectRatio: false,
                scales: { yAxes: [] },
                tooltips: {
                    mode: "single",
                    callbacks: {
                        title: function (tooltipItem, data) { return "Rodada " + data.labels[tooltipItem[0].index]; },
                        label: function (tooltipItems, data) {
                            return data.datasets[tooltipItems.datasetIndex].label + ": " + tooltipItems.yLabel + "º lugar (" + ctrl.evolucao.registrosEvolucao[tooltipItems.datasetIndex].pontuacoes[tooltipItems.index] + "p)";
                        }
                    }
                }
            };

            _(ctrl.evolucao.registrosEvolucao).forEach(function (p) {
                ctrl.graficoEvolucao.series.push(p.nome);
                ctrl.graficoEvolucao.data.push(p.posicoes);
                ctrl.graficoEvolucao.options.scales.yAxes.push(montaEixoParticipacao(p.codigo));
                ctrl.graficoEvolucao.datasetOverride.push(montaOverrideParticipacao(p.codigo));
            });
            _(ctrl.graficoEvolucao.options.scales.yAxes).first().display = true;
            _(ctrl.graficoEvolucao.options.scales.yAxes).first().position = "left";
            _(ctrl.graficoEvolucao.options.scales.yAxes).first().ticks.callback = function (posicao) { return posicao + "º"; };
            _(ctrl.graficoEvolucao.options.scales.yAxes).last().display = true;
            _(ctrl.graficoEvolucao.options.scales.yAxes).last().position = "right";
            _(ctrl.graficoEvolucao.options.scales.yAxes).last().ticks.callback = function (posicao) { return pegaParticipanteNaPosicaoNaUltimaRodada(posicao); };

        }
        // EVOLUCAO - FIM
    }

})();