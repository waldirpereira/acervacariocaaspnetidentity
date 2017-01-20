(function () {
	"use strict";

	angular.module('ui.highlight', []).filter('highlight', function () {
		'use strict';

		return function (text, search, caseSensitive) {
			if (text && (search || angular.isNumber(search))) {
				text = text.toString();
				search = search.toString();
				if (caseSensitive) {
					return text.split(search).join('<span class="ui-match">' + search + '</span>');
				}
				return text.replace(new RegExp(search, 'gi'), '<span class="ui-match">$&</span>');
			}

			return text;
		};
	});

	angular.module("acerva", ["ngSanitize", "ngAnimate", "ngRoute", "ngLocale", "acerva.growl", "ui.bootstrap", "ui.highlight", "ui.select", "frte.datepicker", "png.timeinput"]);
})();