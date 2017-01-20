var DateHelper = function () {
    "use strict";

    return {
        formatoIso: "YYYY-MM-DD",
        formatoDeValorParaDatepicker: "YYYY-MM-DD",
        umDiaEmMilisegundos: 24 * 60 * 60 * 1000,
        formatoData: "DD/MM/YYYY",
        formatoDataHora: "DD/MM/YYYY HH:mm:ss",

        /**
    * Retorna uma nova instância com a soma de "dias" em "data"
    * @param {Date} data data a ter dias adicionados
    * @param {Number} dias número de dias a somar
    * @return {Date} nova instância de Date com a soma realizada
    */
        adicionaDias: function(data, dias) {
            return moment(data).add(dias, "days").toDate();
        },

        /**
    * Retorna o nome do mes baseado no numero do mês, começando em 0 (Jan).
    * @param {Number} mes Mês começando em 0 (jan=0, fev=1, ..., dez=11)
    */
        mesPorExtenso: function(mes) {
            return moment([2012, mes]).format("MMM");
        },

        /**
    * Indica se um ano é bissexto
    * @param {Number} ano
    * @return {Boolean} indica se o ano é bissexto
    */
        anoEhBissexto: function(ano) {
            return moment([ano]).isLeapYear();
        },
        /**
    * Retorna a quantidade de dias no ano
    * @param {Number} ano
    * @return {Number} numero de dias no ano
    */
        quantidadeDiasNoAno: function(ano) {
            return moment([ano]).endOf("year").dayOfYear();
        },

        quantidadeDiasNoAnoPeriodo: function(mesInicio, ano, mesFim) {
            return DateHelper.quantidadeDiasNoAnoPeriodoAnosDiferentes(mesInicio, ano, mesFim, ano);
        },

        quantidadeDiasNoAnoPeriodoAnosDiferentes: function(mesInicial, anoInicial, mesFinal, anoFinal) {
            return moment([anoFinal, mesFinal]).endOf("month")
                .diff(moment([anoInicial, mesInicial]), "days") + 1;
        },

        /**
    * Retorna a quantidade de dias no mês, dependendo do ano (fevereiro varia)
    * @param {Object} mes Mês começando em 0 (jan=0, fev=1, ..., dez=11)
    * @param {Object} ano Ano em que está o mês
    * @return {Number} quantidade de dias
    */
        quantidadeDiasNoMes: function(mes, ano) {
            return moment([ano, mes]).daysInMonth();
        },
        /**
    * Calcula a quantidade de dias entre duas datas
    * @param {Date} dataInicial
    * @param {Date} dataFinal
    * @return {Number} quantidade de dias
    */
        diasAteData: function(dataInicial, dataFinal) {
            return Math.round((dataFinal.getTime() - dataInicial.getTime()) / this.umDiaEmMilisegundos);
        },

        toStringFormatoAcerva: function(data) {
            var dataMoment = moment(data);
            if (dataMoment && dataMoment.isValid())
                return dataMoment.format(DateHelper.formatoData);

            return "";
        },

        toStringFormatoValorParaDatepicker: function(data) {
            var dataMoment = moment(data);
            if (dataMoment && dataMoment.isValid())
                return dataMoment.format(DateHelper.formatoDeValorParaDatepicker);

            return "";
        },

        toStringFormatoDataHora: function(data) {
            var dataMoment = moment(data);
            if (dataMoment && dataMoment.isValid())
                return dataMoment.format(DateHelper.formatoDataHora);

            return "";
        },

        toStringNoLocale: function(stringDateMilisegundosEntreBarras) {
            var dataMoment = moment(stringDateMilisegundosEntreBarras);
            if (dataMoment && dataMoment.isValid())
                return dataMoment.format("L");

            return "";
        },

        toDateFormatoValorParaDatepicker: function (texto) {
            var dataMoment = moment(texto, DateHelper.formatoDeValorParaDatepicker);
            if (dataMoment && dataMoment.isValid())
                return dataMoment.toDate();

            return null;
        },

        toDateDoLocale: function(textoNoLocale) {
            var dataMoment = moment(textoNoLocale, "L");
            if (dataMoment.isValid())
                return dataMoment.toDate();

            return null;
        },

        toDateFormatoAcerva: function(strData) {
            return moment(strData, DateHelper.formatoData).toDate();
        },

        dataEhValida: function(data) {
            return moment(data).isValid();
        },

        saoIguais: function (data1, data2) {
            if ((!data1 && !!data2) || (!!data1 && !data2))
                return false;

            if (!data1 && !data2)
                return true;

            return DateHelper.diasAteData(data1, data2) === 0;
        },

        ehMaiorOuIgual: function (data1, data2) {
            if ((!data1 && !!data2) || (!!data1 && !data2))
                return false;

            if (!data1 && !data2)
                return true;

            return DateHelper.diasAteData(data2, data1) >= 0;
        },

        diaDoMes: function(data) {
            return moment(data).format("D");
        },

        jsonToDate: function(json) {
            return moment(json.substring(0, 10)).add(2, 'hour').toDate(); // horario de verao
        },

        jsonToDateComHora: function(json) {
            return moment(json).toDate();
        },

        meses: function() {
            return moment.months();
        }
    };
}();