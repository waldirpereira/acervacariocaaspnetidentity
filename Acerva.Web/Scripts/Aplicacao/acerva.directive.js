(function () {
    "use strict";

    angular.module("acerva")
        .directive("acervaEnter", [acervaEnter])
        .directive("required", [required])
        .directive("displayUntilDate", [displayUntilDate])
        .directive("acervaConfirm", ["$uibModal", "$timeout", acervaConfirm])
        .directive("acervaUppercase", acervaUppercase)
        .directive("acervaDirtyTracking", ["$window", acervaDirtyTracking])
        .directive('selectOnFocus', selectOnFocus)
        .directive('scrollIf', scrollIf)
        .directive('datepickerFormatter', ["$window", datepickerFormatter])
        .directive('nextOnEnterOrTab', function () {
            return {
                restrict: 'A',
                link: function ($scope, selem, attrs) {
                    selem.bind('keydown', function (e) {
                        e = e || window.event;
                        var code = e.keyCode || e.which;
                        if (code === 13 || code === 9) {
                            e.preventDefault();
                            var pageElems = document.querySelectorAll('input, select, textarea');

                            var elem, evt = e ? e : event;
                            if (evt.srcElement) elem = evt.srcElement;
                            else if (evt.target) elem = evt.target;

                            var focusNext = false;

                            var increment = e.shiftKey ? -1 : 1;

                            for (var i = e.shiftKey ? pageElems.length - 1 : 0; ((i < pageElems.length && !e.shiftKey) || (i >= 0 && e.shiftKey)) ; i += increment) {
                                var pe = pageElems[i];
                                if (focusNext) {
                                    if (pe.style.display !== 'none' && !pe.disabled) {
                                        pe.focus();
                                        break;
                                    }
                                } else if (pe === elem) {
                                    focusNext = true;
                                }
                            }
                        }
                    });
                }
            }
        })
        .directive('dtColumnType', ["DTColumnDefBuilder",
            function (DTColumnDefBuilder) {
                return {
                    restrict: 'A',
                    require: "^ngController",
                    scope: {
                        dtColumnType: '&',
                        dtColumnDefs: '@'
                    },
                    link: function (scope, elem, attrs, modelCtrl) {
                        // informar "disable" no atributo dtColumnType para desabilitar ordenacao

                        if (!modelCtrl.dtColumnDefs)
                            return;

                        var tipo = attrs.dtColumnType;
                        var columnDef = DTColumnDefBuilder.newColumnDef(elem.index());
                        if (tipo === "disabled")
                            columnDef.notSortable();
                        else
                            columnDef.withOption('type', tipo);

                        modelCtrl.dtColumnDefs.push(columnDef);
                    }
                }
            }
        ]);

    function scrollIf() {
        return function (scope, element, attributes) {
            setTimeout(function () {
                function findParentScroll(el) {
                    var overflowY = el.css("overflow-y");  
                    if (overflowY === "scroll" || overflowY === "auto") {
                        return el;
                    } else {
                        if(el.parent().length > 0)
                            return findParentScroll(el.parent());
                        else 
                            return null;
                    }
                }
                if (scope.$eval(attributes.scrollIf)) {
                    var parentWithScroll = findParentScroll(element);
                    if (!parentWithScroll)
                        return;
                    $(parentWithScroll).scrollTop($(parentWithScroll).scrollTop() - $(parentWithScroll).offset().top + $(element).offset().top);
                }
            });
        }
    }

    function selectOnFocus() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.on('focus', function () {
                    this.select();
                });
            }
        }
    }
    function acervaEnter() {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.acervaEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    }

    function required() {
        // http://stackoverflow.com/questions/23275915/angularjs-required-asteriks
        return {
            restrict: "A",
            compile: function (element) {
                element.parent().find("label[for='" + element.attr("name") + "']").first().append("<span>*</span>");
            }
        }
    }

    function displayUntilDate() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var limit = moment(attrs.displayUntilDate, "YYYY-MM-DD").endOf("day");
                if (limit.isValid() && moment.now() > limit)
                    $(element).css("display", "none");
            }
        }
    }

    function acervaConfirm($uibModal, $timeout) {
        // baseado em: https://github.com/venil7/Angular-ui-confirm
        return {
            restrict: "A",
            scope: {
                confirmAction: "&",
                cancelAction: "&",
                confirmText: "@",
                confirmWindowType: "@",
                confirmWhen: "@"
            },
            link: function (scope, elem, attrs) {
                elem.on("click", function () {
                    if (attrs.confirmWhen) {
                        var mustConfirmForm = scope.$parent.$eval(attrs.confirmWhen);
                        if (!mustConfirmForm) {
                            scope.confirmAction && $timeout(scope.confirmAction);
                            return;
                        }
                    }

                    var defaultType = "warning";
                    var modalInstance = $uibModal.open({
                        //templateUrl: '/views/modal.html',
                        windowClass: "bootstrap-dialog size-normal type-" + (scope.confirmWindowType || defaultType),
                        template:
                            "<div class=\"modal-header\"> \
                                <div class=\"bootstrap-dialog-header\"> \
                                    <div class=\"bootstrap-dialog-title\">Confirmação</div> \
                                </div> \
                            </div> \
                        <div class=\"modal-body bootstrap-dialog-message\">{{text}}</div><div class=\"modal-footer\"><button class=\"btn btn-default\" ng-click=\"cancel()\">Não</button><button class=\"btn {{confirmButtonType}}\" ng-click=\"ok()\">Sim</button></div>",
                        controller: ["$scope", "$uibModalInstance", "text", "confirmButtonType", function ($scope, $uibModalInstance, text, confirmButtonType) {
                            $scope.text = text;
                            $scope.confirmButtonType = confirmButtonType;
                            $scope.ok = function () {
                                $uibModalInstance.close();
                            };
                            $scope.cancel = function () {
                                $uibModalInstance.dismiss("cancel");
                            };
                        }],
                        resolve: {
                            text: function () {
                                return scope.confirmText;
                            },
                            confirmButtonType: function () {
                                return "btn-" + (scope.confirmWindowType || defaultType);
                            }
                        }
                    });
                    modalInstance.result.then(function () { scope.confirmAction && scope.confirmAction(); },
                        function () { scope.cancelAction && scope.cancelAction(); }
                    );
                });
            }
        }
    }

    function acervaUppercase() {
        return {
            require: "ngModel",
            link: function (scope, elem, attrs, modelCtrl) {
                function uppercase(input) {
                    if (!input)
                        return input;

                    return input.toUpperCase();
                }

                // altera o modelo
                modelCtrl.$parsers.push(uppercase);

                // altera na tela via css
                elem.css("text-transform", "uppercase");
            }
        };
    }

    //http://stackoverflow.com/questions/14809686/showing-alert-in-angularjs-when-user-leaves-a-page
    // se precisar de uma alternativa, olhar em: https://github.com/facultymatt/angular-unsavedChanges
    function acervaDirtyTracking($window) {
        return {
            restrict: "A",
            link: function ($scope, $element, $attrs) {
                function isDirty() {
                    var formObj = $scope[$element.attr("name")];
                    return formObj && formObj.$pristine === false;
                }

                function areYouSurePrompt() {
                    if (isDirty()) {
                        return "Existem alterações não salvas. Tem certeza que deseja sair desta página?";
                    }
                }

                $($window).bind("beforeunload", areYouSurePrompt);

                $element.bind("$destroy", function () {
                    $($window).unbind("beforeunload", areYouSurePrompt);
                });

                $scope.$on("$locationChangeStart", function (event) {
                    var prompt = areYouSurePrompt();
                    if (!event.defaultPrevented && prompt && !confirm(prompt)) {
                        event.preventDefault();
                    }
                });
            }
        };
    }

    // http://stackoverflow.com/a/14475805/4784342
    function datepickerFormatter($window) {
        return {
            require: '^ngModel',
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                var moment = $window.moment;
                var dateFormat = attrs.moDateInput;
                attrs.$observe('moDateInput', function (newValue) {
                    if (dateFormat === newValue || !ctrl.$modelValue) return;
                    dateFormat = newValue;
                    ctrl.$modelValue = new Date(ctrl.$setViewValue);
                });

                ctrl.$formatters.unshift(function (modelValue) {
                    if (!dateFormat || !modelValue) return "";
                    var retVal = moment(modelValue).format(dateFormat);
                    return retVal;
                });

                ctrl.$parsers.unshift(function (viewValue) {
                    var date = moment(viewValue, dateFormat);
                    return (date && date.isValid() && date.year() > 1950) ? date.toDate() : "";
                });
            }
        };
    }
})();