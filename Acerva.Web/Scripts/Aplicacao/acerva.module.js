(function () {
	"use strict";

    angular
        .module("acerva", ["ngSanitize",
            "ngAnimate", 
            "ngRoute", 
            "ngLocale", 
            "acerva.growl",
            "ui.bootstrap",
            "ui.highlight",
            "ui.select", 
            "frte.datepicker",
            "png.timeinput", 
            "LocalStorageModule"]);
})();