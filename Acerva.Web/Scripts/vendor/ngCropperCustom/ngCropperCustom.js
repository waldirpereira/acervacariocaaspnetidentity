(function () {
    'use strict';

    angular.module('ngCropper', ['ng'])
    .directive('ngCropper', ['$q', '$parse', function ($q, $parse) {
        return {
            restrict: 'A',
            scope: {
                options: '=ngCropperOptions',
                proxy: '=ngCropperProxy', // Optional.
                showEvent: '=ngCropperShow',
                hideEvent: '=ngCropperHide'
            },
            link: function (scope, element, atts) {
                var shown = false;

                scope.$on(scope.showEvent, function () {
                    if (shown) return;
                    shown = true;

                    preprocess(scope.options, element[0])
                        .then(function (options) {
                            setProxy(element);
                            element.cropper(options);
                        });
                });

                function setProxy(element) {
                    if (!scope.proxy) return;
                    var setter = $parse(scope.proxy).assign;
                    setter(scope.$parent, element.cropper.bind(element));
                }

                scope.$on(scope.hideEvent, function () {
                    if (!shown) return;
                    shown = false;
                    element.cropper('destroy');
                });

                scope.$watch('options.disabled', function (disabled) {
                    if (!shown) return;
                    if (disabled) element.cropper('disable');
                    if (!disabled) element.cropper('enable');
                });
            }
        };

        function preprocess(options, img) {
            options = options || {};
            var result = $q.when(options); // No changes.
            if (options.maximize) {
                result = maximizeSelection(options, img);
            }
            return result;
        }

        /**
         * Change options to make selection maximum for the image.
         * fengyuanchen/cropper calculates valid selection's height & width
         * with respect to `aspectRatio`.
         */
        function maximizeSelection(options, img) {
            return getRealSize(img).then(function (size) {
                options.data = size;
                return options;
            });
        }

        /**
         * Returns real image size (without changes by css, attributes).
         */
        function getRealSize(img) {
            var defer = $q.defer();
            var size = { height: null, width: null };
            var image = new Image();

            image.onload = function () {
                defer.resolve({ width: image.width, height: image.height });
            }

            image.src = img.src;
            return defer.promise;
        }
    }])
    .service('Cropper', ['$q', function ($q) {
        this.encode = function (blob) {
            var defer = $q.defer();
            var base64;

            var reader = new FileReader();
            reader.onload = function (e) {
                base64 = e.target.result;

                var canvas = document.createElement('canvas');
                var context = canvas.getContext('2d');

                // cteate Image
                var img = new Image();

                img.onload = function () {
                    // set its dimension to rotated size
                    canvas.height = img.height;
                    canvas.width = img.width;

                    var exif = EXIF.readFromBinaryFile(_base64ToArrayBuffer(img.src));
                    _fixOrientation(canvas, context, exif.Orientation);
                    context.drawImage(img, 0, 0);

                    defer.resolve(canvas.toDataURL("image/png", 100));
                };

                img.src = base64;
            };
            reader.readAsDataURL(blob);
            return defer.promise;
        };

        this.decode = function (dataUrl) {
            var meta = dataUrl.split(';')[0];
            var type = meta.split(':')[1];
            var binary = atob(dataUrl.split(',')[1]);
            var array = new Uint8Array(binary.length);
            for (var i = 0; i < binary.length; i++) {
                array[i] = binary.charCodeAt(i);
            }
            return new Blob([array], { type: type });
        };

        this.crop = function (file, data) {
            var _decodeBlob = this.decode;
            return this.encode(file)
                .then(function (base64) {
                    return _createImage(base64);
                })
                .then(function (image) {
                    var canvas = createCanvas(data);
                    var context = canvas.getContext('2d');
                    
                    context.drawImage(image, data.x, data.y, data.width, data.height, 0, 0, data.width, data.height);

                    var encoded = canvas.toDataURL(file.type);
                    removeElement(canvas);

                    return _decodeBlob(encoded);
                });
        };

        this.scale = function (file, data) {
            var _decodeBlob = this.decode;
            return this.encode(file)
                .then(function (base64) {
                    return _createImage(base64);
                })
                .then(function (image) {
                    var heightOrig = image.height;
                    var widthOrig = image.width;
                    var ratio, height, width;

                    if (angular.isNumber(data)) {
                        ratio = data;
                        height = heightOrig * ratio;
                        width = widthOrig * ratio;
                    }

                    if (angular.isObject(data)) {
                        ratio = widthOrig / heightOrig;
                        height = data.height;
                        width = data.width;

                        if (height && !width)
                            width = height * ratio;
                        else if (width && !height)
                            height = width / ratio;
                    }

                    var canvas = createCanvas(data);
                    var context = canvas.getContext('2d');

                    canvas.width = width;
                    canvas.height = height;
                    
                    context.drawImage(image, 0, 0, widthOrig, heightOrig, 0, 0, width, height);

                    var encoded = canvas.toDataURL(file.type);
                    removeElement(canvas);

                    return _decodeBlob(encoded);
                });
        };

        function _fixOrientation(canvas, context, orientation) {
            if ([5, 6, 7, 8].indexOf(orientation) > -1) {
                var tempWidth = canvas.width;
                canvas.width = canvas.height;
                canvas.height = tempWidth;
            }

            var width = canvas.width;
            var height = canvas.height;

            switch (orientation) {
                case 2: context.transform(-1, 0, 0, 1, width, 0); break;
                case 3: context.transform(-1, 0, 0, -1, width, height); break;
                case 4: context.transform(1, 0, 0, -1, 0, height); break;
                case 5: context.transform(0, 1, 1, 0, 0, 0); break;
                case 6: context.transform(0, 1, -1, 0, height, 0); break;
                case 7: context.transform(0, -1, -1, 0, height, width); break;
                case 8: context.transform(0, -1, 1, 0, 0, width); break;
                default: context.transform(1, 0, 0, 1, 0, 0);
            }
        }

        function _base64ToArrayBuffer(base64) {
            base64 = base64.replace(/^data\:([^\;]+)\;base64,/gmi, '');
            var binaryString = atob(base64);
            var len = binaryString.length;
            var bytes = new Uint8Array(len);
            for (var i = 0; i < len; i++) {
                bytes[i] = binaryString.charCodeAt(i);
            }
            return bytes.buffer;
        }

        function _createImage(source) {
            var defer = $q.defer();
            var image = new Image();
            image.onload = function (e) { defer.resolve(e.target); };
            image.src = source;
            return defer.promise;
        }

        function createCanvas(data) {
            var canvas = document.createElement('canvas');
            canvas.width = data.width;
            canvas.height = data.height;
            canvas.style.display = 'none';
            document.body.appendChild(canvas);
            return canvas;
        }

        function removeElement(el) {
            el.parentElement.removeChild(el);
        }

    }]);

})();
