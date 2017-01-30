using System.Web.Optimization;

namespace Acerva.Web
{
    public class BundleConfig
    {
        #region PASTAS
        private const string ScriptsFolder = "~/Scripts/";
        private const string ScriptsVendorFolder = ScriptsFolder + "vendor/";
        private const string ScriptsAplicacaoFolder = ScriptsFolder + "Aplicacao/";
        private const string AngularExtensionsScriptsFolder = ScriptsAplicacaoFolder + "ngExtensions/";

        private const string StylesFolder = "~/Content/";
        private const string StylesVendorFolder = StylesFolder + "vendor/";
        private const string StylesAplicacaoFolder = StylesFolder + "Aplicacao/";
        public const string BundleStylesDataTables = StylesVendorFolder + "DataTables/media/css/datatables";
        #endregion

        private const string LayoutJs = ScriptsAplicacaoFolder + "_Layout.js";
        private const string DateHelper = ScriptsAplicacaoFolder + "DateHelper.js";

        #region BIBLIOTECAS Utilitárias
        private const string JQuery = ScriptsVendorFolder + "jquery-1.10.2.js";
        private const string Lodash = ScriptsVendorFolder + "lodash.js";
        private static readonly string[] Moment = {
            ScriptsVendorFolder + "moment.js",
            ScriptsVendorFolder + "moment-with-locales.js"
        };
        private static readonly string[] Bootstrap = {
            ScriptsVendorFolder + "bootstrap.js",
            ScriptsVendorFolder + "respond.js"
        };
        private const string BootstrapGrowl = ScriptsVendorFolder + "remarkable-bootstrap-notify/bootstrap-growl.js";
        private static readonly string[] BootstrapDatepicker =
        {
            ScriptsVendorFolder + "bootstrap-datepicker/bootstrap-datepicker.js",
            ScriptsVendorFolder + "bootstrap-datepicker/bootstrap-datepicker.pt-BR.min.js"
        };
        private const string Chart = ScriptsVendorFolder + "chart/Chart.js";
        private static readonly string AngularTimeInputJs = ScriptsVendorFolder + "png-time-input/png-time-input.js";
        #endregion

        private const string AngularDatepicker = ScriptsVendorFolder + "frte-ng-datepicker/frte-ng-datepicker.js";
        private const string AngularHotKeys = ScriptsVendorFolder + "angular-hotkeys/hotkeys.js";
        private const string Angular = ScriptsVendorFolder + "angular.js";
        private const string AngularLocalePtBr = ScriptsVendorFolder + "i18n/angular-locale_pt-br.js";
        private const string AngularAnimate = ScriptsVendorFolder + "angular-animate.js";
        private const string AngularRoute = ScriptsVendorFolder + "angular-route.js";
        private const string AngularSanitize = ScriptsVendorFolder + "angular-sanitize.js";
        private const string AngularLocalStorage = ScriptsVendorFolder + "angular-local-storage/angular-local-storage.js";
        private const string AngularBase64Upload = ScriptsVendorFolder + "angular-base64-upload/angular-base64-upload.js";
        private const string AngularCroppie = ScriptsVendorFolder + "ng-croppie/ng-croppie.js";
        private const string AngularChart = ScriptsVendorFolder + "angular-chart/angular-chart.js";


        private static readonly string[] AngularGrowl =
        {
            ScriptsVendorFolder + "angular-growl.js",
            AngularExtensionsScriptsFolder + "acerva.growl.js"
        };

        private static readonly string[] JQueryDatatables =
        {
            ScriptsVendorFolder + "DataTables/jquery.dataTables.js",
            ScriptsVendorFolder + "DataTables/dataTables.bootstrap.js"
        };
        private const string JQueryDatatablesTableTools = ScriptsVendorFolder + "datatables-tabletools/dataTables.tableTools.js";

        private static readonly string[] AngularUi =
        {
            ScriptsVendorFolder + "angular-ui/ui-bootstrap.js",
            ScriptsVendorFolder + "angular-ui/ui-bootstrap-tpls.js"
        };
        
        private const string AngularSelect = ScriptsVendorFolder + "select.js";

        private static readonly string[] AngularDataTables =
        {
            JQueryDatatables[0],
            JQueryDatatables[1],
            ScriptsVendorFolder + "angular-datatables/angular-datatables.js"
        };


        private const string JQueryDatatablesCss = StylesVendorFolder + "DataTables/media/css/dataTables.bootstrap.css";
        private const string BootstrapDatepickerCss = StylesVendorFolder + "bootstrap-datepicker/bootstrap-datepicker.css";

        #region CSS dos plugins de ANGULAR
        private const string AngularGrowlCss = StylesVendorFolder + "angular-growl-v2/angular-growl.min.css";
        private const string AngularSelectCss = StylesVendorFolder + "angular-ui-select/select.css";
        private const string AngularTimeInputCss = StylesVendorFolder + "png-time-input/png-time-input.css";
        private const string AngularHotkeysCss = StylesVendorFolder + "angular-hotkeys/hotkeys.css";
        private const string AngularCroppieCss = StylesVendorFolder + "ng-croppie/ng-croppie.css";
        #endregion

        public static void RegisterBundles(BundleCollection bundles)
        {
            Layout(bundles);

            CadastroRegionais(bundles);
            CadastroUsuarios(bundles);
            Inicio(bundles);
            Indicacoes(bundles);
            Registro(bundles);
            Admin(bundles);

            bundles.Add(new ScriptBundle("~/bundles/admin")
                .Include(Lodash)
                .Include(JQuery)
                .Include(Bootstrap)
                .Include(ScriptsAplicacaoFolder + "Admin/admin.js")
            );


            CssMinifcados(bundles);
        }

       private static void CssMinifcados(BundleCollection bundles)
        {
               bundles.Add(new StyleBundle(BundleStylesDataTables)
                   .Include(JQueryDatatablesCss));
            
            bundles.Add(new LessBundle("~/cssBundles/angularForm")
                .Include(AngularGrowlCss)
                .Include(AngularSelectCss)
            );
        }

        private static void Layout(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            
            bundles.Add(new ScriptBundle("~/bundles/layout")
                .Include(JQuery)
                .Include(Bootstrap)
                .Include(BootstrapGrowl)
                .Include(ScriptsVendorFolder + "modernizr/modernizr.js")
                .Include(Lodash)
                .Include(Moment)
                .Include(LayoutJs)
                .Include(DateHelper)
                .Include(Angular)
                .Include(AngularSanitize)
                .Include(AngularLocalStorage)
                .Include(AngularLocalePtBr)
                .Include(AngularAnimate)
                .Include(AngularRoute)
                .Include(AngularUi)
                .Include(AngularGrowl)
                .Include(AngularSelect)
                .Include(BootstrapDatepicker)
                .Include(AngularDatepicker)
                .Include(AngularTimeInputJs)
                .Include(AngularDataTables)
                .Include(AngularHotKeys)
                .Include(ScriptsAplicacaoFolder + "acerva.module.js")
                .Include(ScriptsAplicacaoFolder + "acerva.service.js")
                .Include(ScriptsAplicacaoFolder + "acerva.filter.js")
                .Include(ScriptsAplicacaoFolder + "acerva.directive.js")
                .Include(ScriptsAplicacaoFolder + "acerva.layout.controller.js")
                .Include(ScriptsAplicacaoFolder + "FlashMessage.Cookie.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/sitecommon")
                .Include(StylesVendorFolder + "bootswatch/flatly/css/bootstrap.css")
                .Include(StylesAplicacaoFolder + "css/site.less")
                .Include(StylesVendorFolder + "animate.css/animate.css")
                .Include(StylesVendorFolder + "bootstrap-dialog/bootstrap-dialog.css")
                .Include(BootstrapDatepickerCss)
                .Include(AngularTimeInputCss)
                .Include(AngularHotkeysCss)
            );
        }

        private static void CadastroRegionais(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Regional/";
            bundles.Add(new ScriptBundle("~/bundles/regional")
                .Include(path + "acerva.regional.module.js")
                .Include(path + "acerva.regional.service.js")
                .Include(path + "acerva.regional.controller.js")
                .Include(path + "acerva.regional.cadastro.controller.js")
            );
        }

        private static void CadastroUsuarios(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "usuario/";
            bundles.Add(new ScriptBundle("~/bundles/usuario")
                .Include(AngularBase64Upload)
                .Include(AngularCroppie)
                .Include(path + "acerva.usuario.module.js")
                .Include(path + "acerva.usuario.service.js")
                .Include(path + "acerva.usuario.controller.js")
                .Include(path + "acerva.usuario.cadastro.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/usuario")
                .Include(AngularCroppieCss)
            );
        }

        private static void Registro(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "registro/";
            bundles.Add(new ScriptBundle("~/bundles/registro")
                .Include(AngularBase64Upload)
                .Include(AngularCroppie)
                .Include(path + "acerva.registro.module.js")
                .Include(path + "acerva.registro.service.js")
                .Include(path + "acerva.registro.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/registro")
                .Include(AngularCroppieCss)
            );
        }

        private static void Inicio(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Inicio/";
            bundles.Add(new ScriptBundle("~/bundles/inicio")
                .Include(path + "acerva.inicio.module.js")
                .Include(path + "acerva.inicio.service.js")
                .Include(path + "acerva.inicio.controller.js")
            );
        }

        private static void Indicacoes(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Indicacoes/";
            bundles.Add(new ScriptBundle("~/bundles/indicacoes")
                .Include(path + "acerva.indicacoes.module.js")
                .Include(path + "acerva.indicacoes.service.js")
                .Include(path + "acerva.indicacoes.controller.js")
            );
        }

        private static void Admin(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/cssBundles/admin")
                .Include(StylesAplicacaoFolder + "css/admin.css")
            );
        }
    }
}
