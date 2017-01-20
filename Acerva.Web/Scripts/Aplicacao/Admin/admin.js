var Admin = function() {
    "use strict";

    var params;

    function habilitaBundles(habilita) {
        var statusBundles = $("#statusBundles");
        statusBundles.text("Aguarde...");
        $.ajax(params.AlteraHabilitacaoBundles, {
            data: { habilita: habilita },
            success: function(status) {
                statusBundles.text(status);
            }
        });
    }

    function configuraLinksParaTabs() {
        if (!window.location.hash)
            window.location.hash = "#tabLogs";

        // Javascript to enable link to tab
        $(".nav-pills a[href=\"" + window.location.hash + "\"]").tab("show");

        // Change hash for page-reload
        $(".nav-pills a").on("shown.bs.tab", function(e) {
            window.location.hash = e.target.hash;
            window.scrollTo(0, 0);
        });
    }

    return {
        configura: function (p) {
            params = p;
            configuraLinksParaTabs();

            $("#btnHabilitaBundles").click(habilitaBundles.bind(null, true));
            $("#btnDesabilitaBundles").click(habilitaBundles.bind(null, false));

            $("#btnMostraVersaoAspNet").click(function() {
                var divVersaoAspNet = $("#divVersaoAspNet");
                divVersaoAspNet.text("Aguarde...");
                $.ajax(params.MostraVersaoAspNet)
                    .done(function(data) {
                        divVersaoAspNet.text(data);
                    })
                    .error(function(jqXhr, textStatus, errorThrown) {
                        alert("Erro ao recuperar versão Asp Net!\n\ntextStatus: " + textStatus + "\nerrorThrown: " + errorThrown);
                        alert(jqXhr.responseText);
                    });
            });

            $("#btnLimpaCache").click(function() {
                if (!confirm("Tem certeza que deseja limpar o cache de segundo nível do NHibernate?"))
                    return;

                $.ajax(params.LimpaCacheDeSegundoNivelDoNHibernate)
                    .done(function() {
                        alert("O cache foi limpo");
                    })
                    .error(function(jqXhr, textStatus, errorThrown) {
                        alert("Erro ao limpar cache!\n\ntextStatus: " + textStatus + "\nerrorThrown: " + errorThrown);
                        alert(jqXhr.responseText);
                    });
            });

            return {};

        }
    };
}();