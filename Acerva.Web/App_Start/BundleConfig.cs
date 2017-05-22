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
        //private static readonly string[] BootstrapDateTimePicker =
        //{
        //    ScriptsVendorFolder + "bootstrap-datetimepicker/bootstrap-datetimepicker.min.js",
        //    ScriptsVendorFolder + "bootstrap-datetimepicker/pt-br.js"
        //};
        private const string Chart = ScriptsVendorFolder + "chart/Chart.js";
        private static readonly string AngularTimeInputJs = ScriptsVendorFolder + "png-time-input/png-time-input.js";
        #endregion

        private const string AngularDatepicker = ScriptsVendorFolder + "frte-ng-datepicker/frte-ng-datepicker.js";
        private static readonly string[] AngularDateTimePicker =
        {
            ScriptsVendorFolder + "angular-bootstrap-datetimepicker/datetimepicker.js",
            ScriptsVendorFolder + "angular-bootstrap-datetimepicker/datetimepicker.templates.js"
        };

        private const string AngularDateTimeInput = ScriptsVendorFolder + "angular-date-time-input/dateTimeInput.js";
        private const string AngularHotKeys = ScriptsVendorFolder + "angular-hotkeys/hotkeys.js";
        private const string Angular = ScriptsVendorFolder + "angular.js";
        private const string AngularLocalePtBr = ScriptsVendorFolder + "i18n/angular-locale_pt-br.js";
        private const string AngularAnimate = ScriptsVendorFolder + "angular-animate.js";
        private const string AngularRoute = ScriptsVendorFolder + "angular-route.js";

        //SUBSTITUIDO PELO AngularTextAngularSanitize
        //private const string AngularSanitize = ScriptsVendorFolder + "angular-sanitize.js";

        private const string AngularLocalStorage = ScriptsVendorFolder + "angular-local-storage/angular-local-storage.js";
        private const string AngularBase64Upload = ScriptsVendorFolder + "angular-base64-upload/angular-base64-upload.js";
        private static readonly string[] AngularCropper =
        {
            ScriptsVendorFolder + "cropper/cropper.js",
            ScriptsVendorFolder + "ngCropperCustom/ngCropperCustom.js"
        };
        private const string AngularChecklistModel = ScriptsVendorFolder + "checklist-model/checklist-model.js";
        private const string AngularChart = ScriptsVendorFolder + "angular-chart/angular-chart.js";
        private const string AngularTextAngularCore = ScriptsVendorFolder + "text-angular/textAngular.min.js";
        private const string AngularTextAngularSanitize = ScriptsVendorFolder + "text-angular/textAngular-sanitize.js";
        private const string AngularTextAngularRangy = ScriptsVendorFolder + "text-angular/textAngular-rangy.min.js";
        private const string Cpf = ScriptsVendorFolder + "cpf_cnpj/cpf.js";
        private const string Cnpj = ScriptsVendorFolder + "cpf_cnpj/cnpj.js";
        private const string AngularCpfCnpj = ScriptsVendorFolder + "ng-cpf-cnpj/ngCpfCnpj.js";
        private const string AngularUiMask = ScriptsVendorFolder + "angular-ui-mask/mask.js";
        private const string AngularPrint = ScriptsVendorFolder + "angularPrint/angularPrint.js";
        private const string Exif = ScriptsVendorFolder + "exif/exif.js";

        private static readonly string[] AngularGrowl =
        {
            ScriptsVendorFolder + "angular-growl.js",
            AngularExtensionsScriptsFolder + "acerva.growl.js"
        };

        private static readonly string[] JQueryDatatables =
        {
            ScriptsVendorFolder + "DataTables/jquery.dataTables.js",
            ScriptsVendorFolder + "DataTables/dataTables.bootstrap.js",
            ScriptsVendorFolder + "DataTables/dataTables.buttons.js",
            ScriptsVendorFolder + "DataTables/buttons.flash.js",
            ScriptsVendorFolder + "DataTables/buttons.html5.js"
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
            JQueryDatatables[2],
            JQueryDatatables[3],
            ScriptsVendorFolder + "angular-datatables/angular-datatables.js",
            ScriptsVendorFolder + "angular-datatables/angular-datatables.buttons.js"
        };


        private const string JQueryDatatablesCss = StylesVendorFolder + "DataTables/media/css/dataTables.bootstrap.css";
        private const string JQueryDatatablesButtonsCss = StylesVendorFolder + "DataTables/media/css/buttons.dataTables.min.css";
        private const string JQueryDatatablesButtonsSwf = StylesVendorFolder + "DataTables/media/sfw/flashExport.swf";
        private const string BootstrapDatepickerCss = StylesVendorFolder + "bootstrap-datepicker/bootstrap-datepicker.css";
        private const string AngularDateTimePickerCss = StylesVendorFolder + "angular-bootstrap-datetimepicker/datetimepicker.css";

        #region CSS dos plugins de ANGULAR
        private const string AngularGrowlCss = StylesVendorFolder + "angular-growl-v2/angular-growl.min.css";
        private const string AngularSelectCss = StylesVendorFolder + "angular-ui-select/select.css";
        private const string AngularTimeInputCss = StylesVendorFolder + "png-time-input/png-time-input.css";
        private const string AngularHotkeysCss = StylesVendorFolder + "angular-hotkeys/hotkeys.css";
        private static readonly string[] AngularCropperCss =
        {
            StylesVendorFolder + "cropper/cropper.css"
        };
        private const string FontAwesomeCss = StylesVendorFolder + "font-awesome/css/font-awesome.css";
        private const string AngularTextAngularCss = StylesVendorFolder + "text-angular/textAngular.css";
        private const string AngularPrintCss = StylesVendorFolder + "angularPrint/angularPrint.css";
        #endregion

        public static void RegisterBundles(BundleCollection bundles)
        {
            Layout(bundles);

            Acervo(bundles);
            Estatuto(bundles);
            CartilhaBoasVindas(bundles);
            ListaRegionais(bundles);
            ListaBeneficios(bundles);
            CadastroArtigos(bundles);
            CadastroRegionais(bundles);
            CadastroNoticias(bundles);
            CadastroVotacoes(bundles);
            CadastroCategoriasArtigos(bundles);
            CadastroUsuarios(bundles);
            VisualizacaoSituacao(bundles);
            Carteirinha(bundles);
            Inicio(bundles);
            Indicacoes(bundles);
            Registro(bundles);
            MeusDados(bundles);
            CadastroBeneficios(bundles);
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
                .Include(JQueryDatatablesCss)
                .Include(JQueryDatatablesButtonsCss)
                .Include(JQueryDatatablesButtonsSwf)
            );

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
                .Include(AngularTextAngularSanitize)
                .Include(AngularLocalStorage)
                .Include(AngularLocalePtBr)
                .Include(AngularAnimate)
                .Include(AngularRoute)
                .Include(AngularUi)
                .Include(AngularGrowl)
                .Include(AngularSelect)
                .Include(BootstrapDatepicker)
                .Include(AngularDatepicker)
                .Include(AngularDateTimeInput)
                .Include(AngularDateTimePicker)
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
                .Include(AngularDateTimePickerCss)
                .Include(AngularTimeInputCss)
                .Include(AngularHotkeysCss)
            );
        }

        private static void Acervo(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Acervo/";
            bundles.Add(new ScriptBundle("~/bundles/acervo")
                .Include(AngularPrint)
                .Include(path + "acerva.acervo.module.js")
                .Include(path + "acerva.acervo.service.js")
                .Include(path + "acerva.acervo.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/acervo")
                .Include(AngularPrintCss)
            );
        }

        private static void Estatuto(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Estatuto/";
            bundles.Add(new ScriptBundle("~/bundles/estatuto")
                .Include(path + "acerva.estatuto.module.js")
                .Include(path + "acerva.estatuto.service.js")
                .Include(path + "acerva.estatuto.controller.js")
            );
        }

        private static void CartilhaBoasVindas(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "CartilhaBoasVindas/";
            bundles.Add(new ScriptBundle("~/bundles/cartilhaBoasVindas")
                .Include(path + "acerva.cartilhaBoasVindas.module.js")
                .Include(path + "acerva.cartilhaBoasVindas.service.js")
                .Include(path + "acerva.cartilhaBoasVindas.controller.js")
            );
        }

        private static void ListaRegionais(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "ListaRegionais/";
            bundles.Add(new ScriptBundle("~/bundles/listaRegionais")
                .Include(path + "acerva.listaRegionais.module.js")
                .Include(path + "acerva.listaRegionais.service.js")
                .Include(path + "acerva.listaRegionais.controller.js")
            );
        }

        private static void ListaBeneficios(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "ListaBeneficios/";
            bundles.Add(new ScriptBundle("~/bundles/listaBeneficios")
                .Include(path + "acerva.listaBeneficios.module.js")
                .Include(path + "acerva.listaBeneficios.service.js")
                .Include(path + "acerva.listaBeneficios.controller.js")
            );
        }

        private static void CadastroArtigos(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Artigo/";
            bundles.Add(new ScriptBundle("~/bundles/artigo")
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(path + "acerva.artigo.module.js")
                .Include(path + "acerva.artigo.service.js")
                .Include(path + "acerva.artigo.controller.js")
                .Include(path + "acerva.artigo.cadastro.controller.js")
                .Include(path + "acerva.artigo.anexos.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/artigo")
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void CadastroRegionais(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Regional/";
            bundles.Add(new ScriptBundle("~/bundles/regional")
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(path + "acerva.regional.module.js")
                .Include(path + "acerva.regional.service.js")
                .Include(path + "acerva.regional.controller.js")
                .Include(path + "acerva.regional.cadastro.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/regional")
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void CadastroNoticias(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Noticia/";
            bundles.Add(new ScriptBundle("~/bundles/noticia")
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(path + "acerva.noticia.module.js")
                .Include(path + "acerva.noticia.service.js")
                .Include(path + "acerva.noticia.controller.js")
                .Include(path + "acerva.noticia.cadastro.controller.js")
                .Include(path + "acerva.noticia.anexos.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/noticia")
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void CadastroVotacoes(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Votacao/";
            bundles.Add(new ScriptBundle("~/bundles/votacao")
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(path + "acerva.votacao.module.js")
                .Include(path + "acerva.votacao.service.js")
                .Include(path + "acerva.votacao.controller.js")
                .Include(path + "acerva.votacao.cadastro.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/votacao")
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void CadastroCategoriasArtigos(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "CategoriaArtigo/";
            bundles.Add(new ScriptBundle("~/bundles/categoriaArtigo")
                .Include(path + "acerva.categoriaArtigo.module.js")
                .Include(path + "acerva.categoriaArtigo.service.js")
                .Include(path + "acerva.categoriaArtigo.controller.js")
                .Include(path + "acerva.categoriaArtigo.cadastro.controller.js")
            );
        }

        private static void VisualizacaoSituacao(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Situacao/";
            bundles.Add(new ScriptBundle("~/bundles/situacao")
                .Include(path + "acerva.situacao.module.js")
                .Include(path + "acerva.situacao.service.js")
                .Include(path + "acerva.situacao.controller.js")
            );
        }
        private static void Carteirinha(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Carteirinha/";
            bundles.Add(new ScriptBundle("~/bundles/carteirinha")
                .Include(AngularPrint)
                .Include(path + "acerva.carteirinha.module.js")
                .Include(path + "acerva.carteirinha.service.js")
                .Include(path + "acerva.carteirinha.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/carteirinha")
                .Include(AngularPrintCss)
            );
        }

        private static void CadastroUsuarios(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "usuario/";
            bundles.Add(new ScriptBundle("~/bundles/usuario")
                .Include(AngularBase64Upload)
                .Include(Exif)
                .Include(AngularCropper)
                .Include(AngularChecklistModel)
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(Cpf)
                .Include(Cnpj)
                .Include(AngularCpfCnpj)
                .Include(AngularUiMask)
                .Include(path + "acerva.usuario.module.js")
                .Include(path + "acerva.usuario.service.js")
                .Include(path + "acerva.usuario.controller.js")
                .Include(path + "acerva.usuario.cadastro.controller.js")
                .Include(path + "acerva.usuario.selecaoEmails.controller.js")
                .Include(path + "acerva.usuario.historicoStatus.controller.js")
                .Include(path + "acerva.usuario.trocaFoto.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/usuario")
                .Include(AngularCropperCss)
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void Registro(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "registro/";
            const string pathUsuario = ScriptsAplicacaoFolder + "usuario/";
            bundles.Add(new ScriptBundle("~/bundles/registro")
                .Include(AngularBase64Upload)
                .Include(Exif)
                .Include(AngularCropper)
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(Cpf)
                .Include(Cnpj)
                .Include(AngularCpfCnpj)
                .Include(AngularUiMask)
                .Include(path + "acerva.registro.module.js")
                .Include(path + "acerva.registro.service.js")
                .Include(path + "acerva.registro.controller.js")
                .Include(pathUsuario + "acerva.usuario.trocaFoto.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/registro")
                .Include(AngularCropperCss)
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
            );
        }

        private static void MeusDados(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "meusdados/";
            const string pathUsuario = ScriptsAplicacaoFolder + "usuario/";
            bundles.Add(new ScriptBundle("~/bundles/meusdados")
                .Include(AngularBase64Upload)
                .Include(Exif)
                .Include(AngularCropper)
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(Cpf)
                .Include(Cnpj)
                .Include(AngularCpfCnpj)
                .Include(AngularUiMask)
                .Include(path + "acerva.meusdados.module.js")
                .Include(path + "acerva.meusdados.service.js")
                .Include(path + "acerva.meusdados.controller.js")
                .Include(pathUsuario + "acerva.usuario.trocaFoto.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/meusdados")
                .Include(AngularCropperCss)
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
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

        private static void CadastroBeneficios(BundleCollection bundles)
        {
            const string path = ScriptsAplicacaoFolder + "Beneficio/";
            bundles.Add(new ScriptBundle("~/bundles/beneficio")
                .Include(AngularTextAngularRangy)
                .Include(AngularTextAngularCore)
                .Include(path + "acerva.beneficio.module.js")
                .Include(path + "acerva.beneficio.service.js")
                .Include(path + "acerva.beneficio.controller.js")
                .Include(path + "acerva.beneficio.cadastro.controller.js")
            );

            bundles.Add(new LessBundle("~/cssBundles/beneficio")
                .Include(AngularTextAngularCss)
                .Include(FontAwesomeCss)
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
