AcervaApp.Cookie = function () {
    "use strict";

    function getHostName(addPort) {
        return addPort ? window.location.host : window.location.hostname;
    }

    function getSiteUrl() {
        return window.location.protocol + "//" + getHostName(true) + "/Acerva";
    }

    return {
        addCookie: function (cookieName, expireDate, params) {
            try {
                this.removeCookie(cookieName);
                var fullDomainUrl = getSiteUrl().toLowerCase();
                var internalCookieName = cookieName + "@" + fullDomainUrl;

                var endDate = new Date();
                var dateToSet = !expireDate ? (new Date()).getDate() + 1 : expireDate;
                endDate.setDate(dateToSet);

                var cookieValue = "";
                jQuery.each(params, function (idx, val) {
                    cookieValue += "'" + idx + "':'" + escape(val) + "',";
                });

                cookieValue = cookieValue.replace(/(\s+)?.$/, ""); //remove ultimo caractere
                cookieValue = "({" + cookieValue + "})";
                cookieValue += "; expires=" + endDate.toUTCString() + "; path=/";

                document.cookie = internalCookieName + "=" + cookieValue;
            }
            catch (e) {
                return null;
            }
            return null;
        },

        readCookie: function (cookieName) {
            var fullDomainUrl = getSiteUrl().toLowerCase();
            var internalCookieName = cookieName + "@" + fullDomainUrl;

            var aCookies = document.cookie.split(';');
            var foundItem = false;

            for (var i = 0; i < aCookies.length; i++) {

                var item = aCookies[i].replace(/^\s+/, "");
                item = item.replace(/[+]/g, " ");

                if (item.indexOf(internalCookieName) == 0 && item.length > 0)
                    foundItem = true;
                else if (item.toLowerCase().indexOf(internalCookieName) == 0 && item.length > 0)
                    foundItem = true;

                //TODO: test jQuery.parseJSON( json ) method
                if (foundItem) {
                    var valor = item.substring(internalCookieName.length + 1, item.length);
                    var valorComVirgula = valor.replace(/[\^]/g, ",");
                    valorComVirgula = valorComVirgula.replace(/%3A/g, ":");

                    var mensagens = (0, eval)(valorComVirgula);

                    return mensagens;
                }
            }
            return null;
        },

        removeCookie: function (cookieName) {
            var fullDomainUrl = getSiteUrl();
            fullDomainUrl = fullDomainUrl.toLowerCase();
            var internalCookieName = cookieName + "@" + fullDomainUrl;
            document.cookie = internalCookieName + "=; expires=; path=/";
        }
    };
}(AcervaApp.Cookie || {}, jQuery);

AcervaApp.FlashMessage = function () {
    "use strict";

    $.growl(false, {
        allow_dismiss: true,
        mouse_over: "pause"
    });

    var cookieName = 'AcervaMessage';
    var deParaAcervaGrowlTypes = {
        error: ["danger", "icon-error-alt"],
        info: ["info", "icon-info-circled"],
        warning: ["warning", "icon-attention"],
        forbidden: ["danger", "icon-block"],
        success: ["success", "icon-ok-circle"]
    }

    function readMessageFromCookie(cookie) {
        return {
            title: unescape(cookie.messageTitle),
            text: unescape(cookie.messageValue),
            type: deParaAcervaGrowlTypes[unescape(cookie.messageType)][0],
            sticky: (cookie.userMustCloseMessage)
        };
    }

    return {
        showFromCookie: function () {
            var mensagens = AcervaApp.Cookie.readCookie(cookieName) || [];

            var existSystemMessage = mensagens.length > 0;
            for (var i = 0; i < mensagens.length; i++) {
                var message = readMessageFromCookie(mensagens[i]);
                AcervaApp.FlashMessage.message(message);
            }

            if (existSystemMessage) {
                AcervaApp.Cookie.removeCookie(cookieName);
            }
        },

        message: function (mensagem) {
            var paramType = mensagem.type ? unescape(mensagem.type) : "info";
            var type = deParaAcervaGrowlTypes[paramType][0];
            var icon = deParaAcervaGrowlTypes[paramType][1];

            $.growl({
                icon: icon,
                title: "<strong>" + mensagem.title + "</strong><br/>",
                message: mensagem.text
            }, {
                delay: mensagem.sticky ? 10000000000 : 5000,
                type: type,
                animate: {
                    enter: 'animated bounceInDown',
                    exit: 'animated bounceOutRight'
                },
                z_index: 9999999,
                template: '<div data-growl="container" class="alert col-sm-3" role="alert">'
		                    + '<button type="button" class="close" data-growl="dismiss">'
			                + '    <span aria-hidden="true">×</span>'
			                + '    <span class="sr-only">Close</span>'
		                    + ' </button>'
                            + '        <div>'
		                    + '            <div style="float: left;width: 50px;">'
			                + '                <span data-growl="icon" style="font-size: 2.5em;"></span> '
		                    + '            </div>'
		                    + '            <div style="margin-left: 50px;"> '
			                + '                <span data-growl="title" style="font-size: 1.2em;"></span>'
                            + '                <span data-growl="message" style="font-size: 1em; margin-top: 5px; display: inline-block;"></span>'
                            + '                <a href="#" data-growl="url"></a>'
		                    + '            </div>'
	                        + '        </div>'
	                        + '</div>'
            });
        }

    };
}(AcervaApp.FlashMessage || {}, jQuery);