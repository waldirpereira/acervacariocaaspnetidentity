(function () {
	"use strict";

    var app = angular
        .module("acerva", [
            "ngSanitize",
            "ngAnimate", 
            "ngRoute", 
            "ngLocale", 
            "acerva.growl",
            "ui.bootstrap",
            "ui.highlight",
            "ui.select", 
            "frte.datepicker",
            "png.timeinput", 
            "LocalStorageModule"
        ]);

    app.config(function(localStorageServiceProvider) {
        localStorageServiceProvider.setPrefix("acerva");
    });
})();