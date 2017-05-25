window.requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame ||
                              window.webkitRequestAnimationFrame || window.msRequestAnimationFrame || function (c) { setTimeout(c); };

var AcervaApp = {};

AcervaApp.Layout = function () {
    "use strict";

    var CODIGO_ENTER = 13;
    
    return {
        params: null,
        inicializaPagina: inicializaPagina,

        configuraMenu: configuraMenu,

        pegaMensagemDeErroDoXhr: pegaMensagemDeErroDoXhr,
        retornoDeChamadaAjaxComErro: retornoDeChamadaAjaxComErro,
        trataErro: trataErro,

        resetCombo: resetCombo,

        showProgress: showProgress,
        hideProgress: hideProgress,
        criaDataTable: criaDataTable,
        criaDualList: criaDualList,
        
        configuraDatepicker: configuraDatepicker,
        setDatepickerValue: setDatepickerValue,

        modalAsWindow: modalAsWindow,

        configuraLocaleBr: configuraLocalePtBr
    };

    // funções PÚBLICAS

    function inicializaPagina(p) {
        AcervaApp.Layout.params = p;
        AcervaApp.user = AcervaApp.Layout.params.user;
        
        $('body').ajaxSuccess(function (e, xhr) {
            if (xhr.status === 302) {
                location.href = xhr.getResponseHeader("Location");
            }
        });

        if (!(AcervaApp.FlashMessage == undefined)) {
            AcervaApp.FlashMessage.showFromCookie();
        }
        
        permiteApenasTabNosGroupButtonsDeRadioButtonsOuCheckBoxes();
        configuraLocalePtBr();

        if ($.fn.datepicker) {
            $.fn.datepicker.defaults = $.extend($.fn.datepicker.defaults, {
                format: "dd/mm/yyyy",
                todayBtn: "linked",
                language: "pt-BR",
                autoclose: true,
                todayHighlight: true
            });
        }

        configuraInputsDateComoDatepickers();
        executaConfiguracoesGeraisDosDataTables();
        configuraModalBootstrapParaNaoRemoverClasseQuandoAindaExisteModalAberto();
    }

    function configuraModalBootstrapParaNaoRemoverClasseQuandoAindaExisteModalAberto() {
        //https://github.com/twbs/bootstrap/issues/15260
        // Resolve problema de abrir um modal logo após o fechamento de outro
        if (!$.fn.modal)
            return;

        var show = $.fn.modal.Constructor.prototype.show;

        $.fn.modal.Constructor.prototype.show = function () {
            this.modalOpen = !this.$body.hasClass('modal-open');
            show.apply(this, arguments);
        };

        $.fn.modal.Constructor.prototype.hideModal = function () {
            var that = this;
            this.$element.hide();
            this.backdrop(function () {
                if (that.modalOpen) {
                    that.$body.removeClass('modal-open');
                }
                that.resetScrollbar();
                that.$element.trigger('hidden.bs.modal');
            });
        };
    }
    
    function configuraMenu(locatorOpcao, actionLink, target) {
        $(locatorOpcao).attr("href", actionLink);
        if (target)
            $(locatorOpcao).attr("target", target);
    }

    function pegaMensagemDeErroDoXhr(jqXhr, errorThrown) {
        var erro = null;
        try {
            if (jqXhr.responseText) {
                var responseTextMin = jqXhr.responseText.toLowerCase();
                var inicioTitle = responseTextMin.indexOf("<title>");
                if (inicioTitle >= 0) {
                    var fimTitle = responseTextMin.indexOf("</title>");
                    return jqXhr.responseText.substring(inicioTitle + "<title>".length, fimTitle);
                } else {
                    return $.parseJSON(jqXhr.responseText).message;
                }
            }
            erro = $.parseJSON(jqXhr.responseText).message;
        } catch (ex) {
            if (!erro || erro.length === 0) {
                return errorThrown;
            }
        }

        return erro;
    }

    function retornoDeChamadaAjaxComErro(jqXhr, textStatus, errorThrown) {
        $("button.disabled[data-loading-text]").button('reset');

        AcervaApp.Layout.hideProgress();
        
        var mensagem = AcervaApp.Layout.pegaMensagemDeErroDoXhr(jqXhr, errorThrown);

        AcervaApp.FlashMessage.message({
            title: 'Erro na solicitação',
            text: mensagem,
            type: 'error',
            sticky: true
        });
    }

    function trataErro(mensagem, titulo) {
        titulo = titulo || 'Erro';

        AcervaApp.Layout.hideProgress();
        
        AcervaApp.FlashMessage.message({
            title: titulo,
            text: mensagem,
            type: 'error',
            sticky: true
        });
    }

    function resetCombo(combobox, colocaOpcaoSelecione) {
        colocaOpcaoSelecione = colocaOpcaoSelecione != null ? colocaOpcaoSelecione : true;
        var objHtmlSelect = $(combobox);

        objHtmlSelect.empty();
        if (colocaOpcaoSelecione) {
            objHtmlSelect.append($('<option/>', { value: 0, text: "Selecione..." }));
        }
    }

    function showProgress(textoPrincipal, textoSecundario) {
        var textoPadrao = "Aguarde. Processando...";
        textoPrincipal = (typeof textoPrincipal === 'string') ? textoPrincipal || textoPadrao : textoPadrao;

        var dialog = $('#progressDialog');
        $('#progressDialog .modal-title').text(textoPrincipal);

        textoSecundario = textoSecundario || "";
        $('#progressDialog .textoSecundarioProgresso').text(textoSecundario);

        if (!dialog.is(":visible")) dialog.modal();
    }

    function hideProgress() {
        $('#progressDialog').modal('hide');
    }

    function criaDataTable(tableSelector, dataTableSettings) {
        var settings = dataTableSettings || {};

        var table = $(tableSelector);

        if (table.size() === 0) return null;

        var datatable = table
            .removeClass('display')
            .addClass('table table-striped table-bordered table-condensed hover')
            .DataTable(settings);

        if (settings.multifiltering) {
            configuraMultifiltering(table);
        }

        table.css("visibility", "visible").show();

        return datatable;
    }

    function criaDualList(selector, textoDisponiveis, textoSelecionados, options) {
        var lista = $(selector);

        var settings = $.extend({}, {
            nonSelectedListLabel: textoDisponiveis,
            selectedListLabel: textoSelecionados,
            preserveSelectionOnMove: 'moved',
            moveOnSelect: false,
            infoText: '(mostrando {0})',
            infoTextFiltered: '<span class="label label-warning">Filtrando</span> {0} de {1}',
            infoTextEmpty: '(lista vazia)',
            filterPlaceHolder: 'Filtrar',
            filterTextClear: 'mostrar todos',
            moveSelectedLabel: 'mover selecionados',
            moveAllLabel: 'mover todos',
            removeSelectedLabel: 'remover selecionados',
            removeAllLabel: 'remover todos'
        }, options);

        lista.bootstrapDualListbox(settings);

        if (!lista.prop("disabled") && !lista.hasClass("disabled"))
            return;

        lista.siblings(".bootstrap-duallistbox-container").find(":input:not(.filter)").prop("disabled", true);
    }
    function setDatepickerValue(datepicker, date) {
        var $datepicker = $(datepicker);
        $datepicker.val(DateHelper.toStringFormatoValorParaDatepicker(date));

        if (!Modernizr.inputtypes.date && !$datepicker.prop("readonly"))
            $datepicker.datepicker("update", date);
    }

    function modalAsWindow(selectorModal) {
        var modal = $(selectorModal);
        if (modal.hasClass("modal")) {
            var modalDialog = modal.find(".modal-dialog");
            var margem = ($(window).width() / 2) - (modalDialog.width() / 2);

            var novaModal = modalDialog.detach();
            modal.remove();

            novaModal.prop("id", modal.prop("id"));
            novaModal.css("position", "absolute");
            novaModal.css("z-index", "1040");

            novaModal.css("margin-left", margem + "px");
            novaModal.css("margin-right", margem + "px");
            novaModal.css("margin-bottom", "30px");
            novaModal.css("margin-top", "30px");

            novaModal.draggable({ handle: ".modal-header" });
            novaModal.find(".modal-header").css("cursor", "move");
            novaModal.find(".modal-header button.close").click(function () { novaModal.fadeOut(); });

            $("#main").prepend(novaModal);

            modal = novaModal;
        }
        modal.fadeIn();
    }

    // funções PRIVADAS

    function executaConfiguracoesGeraisDosDataTables() {
        if (!$.fn.dataTable)
            return;

        extendeDataTableParaFazerFiltroAtravesDaTeclaEnter();
        extendeDataTableParaFiltrarAtravesDeUmBotao();
        extendeDataTableParaAdicionarTipoDeColunaDataBrasileiraParaOrdenacao();

        $.extend($.fn.dataTable.defaults, {
            autoWidth: true,
            stateSave: true,
            responsive: true,
            language: obtemParametrosDataTablePtBr(),
            columnDefs: [
                {
                    sortable: false,
                    targets: ["sorting_disabled"]
                }
            ]
        });

        $(document).on('init.dt.dtr', function (e) {
            // aplica as extensões sempre que um datatable terminar sua inicialização
            var table = $(e.target);

            table
                .dataTable()
                .fnFilterOnReturn()
                .fnFilterOnButtonClick();

            if (table.attr("dt-multifiltering")) {
                configuraMultifiltering(table);
            }
        });
    }

    function extendeDataTableParaFazerFiltroAtravesDaTeclaEnter() {
        //http://datatables.net/plug-ins/api/fnFilterOnReturn
        //Require the return key to be pressed to filter a table
        if (!jQuery.fn.dataTableExt) return;

        jQuery.fn.dataTableExt.oApi.fnFilterOnReturn = function () {
            var that = this;
            this.each(function (i) {
                $.fn.dataTableExt.iApiIndex = i;
                var anControl = $('input', that.fnSettings().aanFeatures.f);
                anControl
                    .unbind('keyup search input')
                    .bind('keypress', function (e) {
                        if (e.which === CODIGO_ENTER) {
                            $.fn.dataTableExt.iApiIndex = i;
                            that.fnFilter(anControl.val());
                        }
                    });
                return this;
            });
            return this;
        };
    }

    function extendeDataTableParaAdicionarTipoDeColunaDataBrasileiraParaOrdenacao() {
        // https://www.datatables.net/forums/discussion/comment/46858/#Comment_46858
        if (!jQuery.fn.dataTableExt) return;

        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "date-br-pre": function (a) {
                var x = new Date(0, 0, 0, 0, 0, 0, 0).getTime();
                try {
                    var dateA = a.replace(/ /g, '').split("/");
                    if (dateA.length !== 3)
                        return x;

                    var day = parseInt(dateA[0], 10);
                    var month = parseInt(dateA[1], 10);
                    var year = parseInt(dateA[2], 10);
                    var date = new Date(year, month - 1, day);

                    x = date.getTime();
                }
                catch (err) { }
                return x;
            },
            "date-br-asc": function (a, b) {
                return a - b;
            },
            "date-br-desc": function (a, b) {
                return b - a;
            }
        });
    }

    function extendeDataTableParaFiltrarAtravesDeUmBotao() {
        if (!jQuery.fn.dataTableExt) return;

        jQuery.fn.dataTableExt.oApi.fnFilterOnButtonClick = function () {
            var that = this;
            this.each(function (i) {
                $.fn.dataTableExt.iApiIndex = i;

                var tableName = that.attr("id");
                var divContainer = $("#" + tableName + "_filter");
                var inputPesquisa = divContainer.find("input")
                    .attr("placeholder", "Pesquisar...")
                    .detach();

                divContainer.html("");
                divContainer.addClass("input-group pull-right");

                var anButton = $("<button/>")
                    .attr("id", that.attr("id") + "_filter_button")
                    .addClass("btn btn-info glyphicon glyphicon-filter btn-sm")
                    .css("z-index", "2")
                    .css("top", "inherit")
                    .css("line-height", "1.5");
                var spanInputGroupBtn = $("<span/>").addClass("input-group-btn");
                spanInputGroupBtn.append(anButton);

                divContainer.append(inputPesquisa).append(spanInputGroupBtn);

                anButton.click(function () {
                    that.fnFilter(inputPesquisa.val());
                    return false;
                });
                return this;
            });
            return this;
        }
    }

    function permiteApenasTabNosGroupButtonsDeRadioButtonsOuCheckBoxes() {
        $('[data-toggle="buttons"] input[type="checkbox"],[data-toggle="buttons"] input[type="radio"]').bind('keypress', function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode !== 9 /*TAB*/) {
                return false;
            }
            return true;
        });
    }

    function temDateHelper() {
        return typeof (DateHelper) !== "undefined";
    }

    function configuraInputsDateComoDatepickers() {
        var suportaDate = Modernizr.inputtypes.date;

        if (temDateHelper()) {
            DateHelper.formatoDeValorParaDatepicker = suportaDate ? "YYYY-MM-DD" : "DD/MM/YYYY";
        }

        if (suportaDate || !$.fn.datepicker)
            return;

        var inputs = $("input[type='date']");

        inputs.each(function (i, el) {
            var $el = $(el);
            var dataRfc = $el.val();
            if (!dataRfc) return;

            var data = new Date(dataRfc);
            data = new Date(data.getTime() + data.getTimezoneOffset() * 60000);
            $el.val(moment(data).format("DD/MM/YYYY"));
        });

        configuraDatepicker(inputs);
    }

    function configuraDatepicker(datepicker, options) {
        datepicker = $(datepicker);

        if (Modernizr.inputtypes.date) {
            if (!temDateHelper()) return null;

            if (options.startDate)
                datepicker.attr("min", DateHelper.toStringFormatoValorParaDatepicker(options.startDate));
            if (options.endDate)
                datepicker.attr("max", DateHelper.toStringFormatoValorParaDatepicker(options.endDate));

            return null;
        }

        datepicker.datepicker("remove");

        return datepicker.datepicker(options);
    }

    function obtemParametrosDataTablePtBr() {
        // http://datatables.net/plug-ins/i18n/Portuguese-Brasil
        return {
            emptyTable: "Nenhum registro encontrado",
            info: "Mostrando de _START_ até _END_ de _TOTAL_ registros",
            infoEmpty: "Mostrando 0 até 0 de 0 registros",
            infoFiltered: "(Filtrados de _MAX_ registros)",
            infoPostFix: "",
            decimal: ",",
            //"sInfoThousands": ".",
            lengthMenu: "_MENU_ resultados por página",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            zeroRecords: "Nenhum registro encontrado",
            search: "Pesquisar",
            paginate: {
                next: "Próximo",
                previous: "Anterior",
                first: "Primeiro",
                last: "Último"
            },
            aria: {
                sortAscending: ": Ordenar colunas de forma ascendente",
                sortDescending: ": Ordenar colunas de forma descendente"
            }
        };
    }

    function configuraLocalePtBr() {
        if (typeof moment !== "undefined")
            moment.locale("pt-BR");
    }
    
    function configuraMultifiltering(table) {
        if (table.find(".campoFiltroColunaDataTable").size() > 0) {
            return;
        }

        var datatable = table.DataTable();
        var tr = $("<tr />");
        _.forEach(table.find("thead tr th"), function (th, indice) {
            var hasHideClass = $(th).hasClass("hide");
            var hasHiddenClass = $(th).hasClass("hidden");
            var title = $(th).text();
            var value = datatable
                .column(indice)
                .search();
            var input = title.trim() !== "" ? '<input type="text" class="campoFiltroColunaDataTable" placeholder="' + title + '" value="' + value + '"/>' : '';
            var filterCell = $("<th />").html(input);
            hasHideClass && filterCell.addClass("hide");
            hasHiddenClass && filterCell.addClass("hidden");
            tr.append(filterCell);
        });

        table.find("thead").append(tr);

        _.forEach(datatable.columns().eq(0), function (colIdx) {
            table.find("thead tr:nth-child(2) th:nth-child(" + (colIdx + 1) + ") input").on('keyup change', function () {
                datatable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
        });
    }
}();
