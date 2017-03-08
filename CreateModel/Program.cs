using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CreateModel
{
    public class Program
    {
        const string PrefixoSistema = "Acerva";
        const string PrefixoSistemaNg = "acerva";

        public static string ModeloSingular;
        public static string ModeloPlural;
        public static string TituloSingular;
        public static string TituloPlural;
        public static string Artigo = string.Empty;

        static void Main(string[] args)
        {
            Console.Write("Nome do modelo no singular (ex.: Aviao): ");
            while (string.IsNullOrEmpty(ModeloSingular))
                ModeloSingular = Console.ReadLine();

            Console.Write("Nome do modelo no plural (ex.: Avioes): ");
            while (string.IsNullOrEmpty(ModeloPlural))
                ModeloPlural = Console.ReadLine();

            Console.Write("Título do modelo no singular (ex.: Avião): ");
            while (string.IsNullOrEmpty(TituloSingular))
                TituloSingular = Console.ReadLine();

            Console.Write("Título do modelo no plural (ex.: Aviões): ");
            while (string.IsNullOrEmpty(TituloPlural))
                TituloPlural = Console.ReadLine();

            while (Artigo != "a" && Artigo != "o")
            {
                Console.Write("Artigo ('o' ou 'a'): ");
                Artigo = Console.ReadLine();
            }
            
            // modelo
            var pathModelo = string.Format("../../../{0}.Modelo", PrefixoSistema);
            ProcessaArquivo("Modelo/Aviao.cs", Path.Combine(pathModelo, string.Format("{0}.cs", ModeloSingular)));
            ProcessaArquivo("Modelo/Validadores/AviaoValidator.cs", Path.Combine(pathModelo, string.Format("Validadores/{0}Validator.cs", ModeloSingular)));
            AlteraCsproj("Compile", Path.Combine(pathModelo, string.Format("{0}.Modelo.csproj", PrefixoSistema)), new List<string> {
                    string.Format("{0}.cs", ModeloSingular),
                    string.Format("Validadores\\{0}Validator.cs", ModeloSingular)
                });

            // mapeamento
            var pathMapeamento = string.Format("../../../{0}.Modelo.Mapeamento", PrefixoSistema);
            ProcessaArquivo("Modelo.Mapeamento/AviaoClassMap.cs", Path.Combine(pathMapeamento, string.Format("{0}ClassMap.cs", ModeloSingular)));
            AlteraCsproj("Compile", Path.Combine(pathMapeamento, string.Format("{0}.Modelo.Mapeamento.csproj", PrefixoSistema)), new List<string> {
                    string.Format("{0}ClassMap.cs", ModeloSingular)
                });

            // infra
            var pathInfra = string.Format("../../../{0}.Infra", PrefixoSistema);
            ProcessaArquivo("Infra/Repositorios/CadastroAvioes.cs", Path.Combine(pathInfra, string.Format("Repositorios/Cadastro{0}.cs", ModeloPlural)));
            ProcessaArquivo("Infra/Repositorios/ICadastroAvioes.cs", Path.Combine(pathInfra, string.Format("Repositorios/ICadastro{0}.cs", ModeloPlural)));
            AlteraCsproj("Compile", Path.Combine(pathInfra, string.Format("{0}.Infra.csproj", PrefixoSistema)), new List<string> {
                    string.Format("Repositorios\\Cadastro{0}.cs", ModeloPlural),
                    string.Format("Repositorios\\ICadastro{0}.cs", ModeloPlural)
                });

            // web
            var pathWeb = string.Format("../../../{0}.Web", PrefixoSistema);
            ProcessaArquivo("Web/Controllers/AviaoController.cs", Path.Combine(pathWeb, string.Format("Controllers/{0}Controller.cs", ModeloSingular)));

            if (!Directory.Exists(Path.Combine(pathWeb, string.Format("Models/Cadastro{0}", ModeloSingular))))
                Directory.CreateDirectory(Path.Combine(pathWeb, string.Format("Models/Cadastro{0}", ModeloSingular)));
            ProcessaArquivo("Web/Models/CadastroAviao/AviaoViewModel.cs", Path.Combine(pathWeb, string.Format("Models/Cadastro{0}/{0}ViewModel.cs", ModeloSingular)));
            ProcessaArquivo("Web/Models/CadastroAviao/CadastroAvioesMapperProfile.cs", Path.Combine(pathWeb, string.Format("Models/Cadastro{0}/Cadastro{1}MapperProfile.cs", ModeloSingular, ModeloPlural)));

            if (!Directory.Exists(Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}", ModeloSingular))))
                Directory.CreateDirectory(Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}", ModeloSingular)));
            ProcessaArquivo(string.Format("Web/Scripts/Aplicacao/Aviao/{0}.aviao.cadastro.controller.js", PrefixoSistemaNg), Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}/{1}.{2}.cadastro.controller.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant())));
            ProcessaArquivo(string.Format("Web/Scripts/Aplicacao/Aviao/{0}.aviao.controller.js", PrefixoSistemaNg), Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}/{1}.{2}.controller.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant())));
            ProcessaArquivo(string.Format("Web/Scripts/Aplicacao/Aviao/{0}.aviao.module.js", PrefixoSistemaNg), Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}/{1}.{2}.module.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant())));
            ProcessaArquivo(string.Format("Web/Scripts/Aplicacao/Aviao/{0}.aviao.service.js", PrefixoSistemaNg), Path.Combine(pathWeb, string.Format("Scripts/Aplicacao/{0}/{1}.{2}.service.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant())));

            if (!Directory.Exists(Path.Combine(pathWeb, string.Format("Views/{0}", ModeloSingular))))
                Directory.CreateDirectory(Path.Combine(pathWeb, string.Format("Views/{0}", ModeloSingular)));
            ProcessaArquivo("Web/Views/Aviao/_CadastroAviao.cshtml", Path.Combine(pathWeb, string.Format("Views/{0}/_Cadastro{0}.cshtml", ModeloSingular)));
            ProcessaArquivo("Web/Views/Aviao/_ListaAvioes.cshtml", Path.Combine(pathWeb, string.Format("Views/{0}/_Lista{1}.cshtml", ModeloSingular, ModeloPlural)));
            ProcessaArquivo("Web/Views/Aviao/Index.cshtml", Path.Combine(pathWeb, string.Format("Views/{0}/Index.cshtml", ModeloSingular)));

            AlteraCsproj("Compile", Path.Combine(pathWeb, string.Format("{0}.Web.csproj", PrefixoSistema)), new List<string>
            {
                string.Format("Controllers\\{0}Controller.cs", ModeloSingular),
                string.Format("Models/Cadastro{0}\\{0}ViewModel.cs", ModeloSingular),
                string.Format("Models/Cadastro{0}\\Cadastro{1}MapperProfile.cs", ModeloSingular, ModeloPlural)
            });

            AlteraCsproj("Content", Path.Combine(pathWeb, string.Format("{0}.Web.csproj", PrefixoSistema)), new List<string>
            {
                string.Format("Scripts\\Aplicacao\\{0}\\{1}.{2}.cadastro.controller.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant()),
                string.Format("Scripts\\Aplicacao\\{0}\\{1}.{2}.controller.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant()),
                string.Format("Scripts\\Aplicacao\\{0}\\{1}.{2}.module.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant()),
                string.Format("Scripts\\Aplicacao\\{0}\\{1}.{2}.service.js", ModeloSingular, PrefixoSistemaNg, ModeloSingular.ToLowerInvariant()),
                string.Format("Views\\{0}\\_Cadastro{0}.cshtml", ModeloSingular),
                string.Format("Views\\{0}\\_Lista{1}.cshtml", ModeloSingular, ModeloPlural),
                string.Format("Views\\{0}\\Index.cshtml", ModeloSingular)
            });

            // bundle config
            CriaConfiguracaoBundle();

            // ninject
            CriaConfiguracaoNinjectRepositorio();

            // global.asax.cs (ConfiguraAutoMapper)
            ConfiguraAutoMapperNoGlobalAsax();

            Console.Write("Geração terminada com sucesso. Pressione ENTER tecla para finalizar.");
            Console.ReadLine();
        }

        private static void ConfiguraAutoMapperNoGlobalAsax()
        {
            var caminhoCompletoGlobalAsax = string.Format("../../../{0}.Web/Global.asax.cs", PrefixoSistema);
            var conteudoGlobalAsax = File.ReadAllText(caminhoCompletoGlobalAsax);
            var posicaoInclusao = conteudoGlobalAsax.IndexOf("cfg.AddProfile<", StringComparison.CurrentCulture);

            var config = string.Format("cfg.AddProfile<Cadastro{0}MapperProfile>();" + "\n" +
                "                " + "\n                ", ModeloPlural);
            conteudoGlobalAsax = conteudoGlobalAsax.Insert(posicaoInclusao, config);

            File.WriteAllText(caminhoCompletoGlobalAsax, conteudoGlobalAsax.Insert(0, string.Format("using {0}.Web.Models.Cadastro{1};\n", PrefixoSistema, ModeloPlural)));
        }

        private static void CriaConfiguracaoNinjectRepositorio()
        {
            var caminhoCompletoNinjectModule = string.Format("../../../{0}.Web/Ninject/RepositoriosNoBancoDeDadosNinjectModule.cs", PrefixoSistema);
            var conteudoNinjectModule = File.ReadAllText(caminhoCompletoNinjectModule);
            var posicaoInclusao = conteudoNinjectModule.IndexOf("Bind(typeof", StringComparison.CurrentCulture);

            var config = string.Format("Bind(typeof(ICadastro{0}))\n" +
                "                .To(typeof(Cadastro{0}))" + "\n" +
                "                .InRequestScope();" + "\n" +
                "" + "\n                ", ModeloPlural);

            File.WriteAllText(caminhoCompletoNinjectModule, conteudoNinjectModule.Insert(posicaoInclusao, config));
        }

        private static void CriaConfiguracaoBundle()
        {
            var caminhoCompletoBundleConfig = string.Format("../../../{0}.Web/App_Start/BundleConfig.cs", PrefixoSistema);
            var conteudoBundleConfig = File.ReadAllText(caminhoCompletoBundleConfig);

            var posicaoInclusaoChamada = conteudoBundleConfig.IndexOf("Admin(bundles);", StringComparison.CurrentCulture);
            var chamada = string.Format("Cadastro{0}(bundles);\n\t\t\t", ModeloPlural);
            conteudoBundleConfig = conteudoBundleConfig.Insert(posicaoInclusaoChamada, chamada);

            var posicaoInclusaoMetodo = conteudoBundleConfig.IndexOf("private static void Admin(BundleCollection bundles)", StringComparison.CurrentCulture);
            var metodo = string.Format("private static void Cadastro{0}(BundleCollection bundles)" +
                "        {{" + "\n" +
                "                const string path = ScriptsAplicacaoFolder + \"{1}/\"; " + "\n" +
                "                bundles.Add(new ScriptBundle(\"~/bundles/{2}\")" + "\n" +
                "                    .Include(path + \"{3}.{2}.module.js\")" + "\n" +
                "                    .Include(path + \"{3}.{2}.service.js\")" + "\n" +
                "                    .Include(path + \"{3}.{2}.controller.js\")" + "\n" +
                "                    .Include(path + \"{3}.{2}.cadastro.controller.js\")" + "\n" +
                "                ); " + "\n" +
                "            }}" + "\n" +
                "" + "\n            ", ModeloPlural, ModeloSingular, TransformaPrimeiroCaractereParaMinusculo(ModeloSingular), PrefixoSistemaNg);
            conteudoBundleConfig = conteudoBundleConfig.Insert(posicaoInclusaoMetodo, metodo);

            File.WriteAllText(caminhoCompletoBundleConfig, conteudoBundleConfig);
        }

        private static void AlteraCsproj(string tipo, string caminhoCompletoCsproj, IEnumerable<string> nomesArquivos)
        {
            var conteudoCsproj = File.ReadAllText(caminhoCompletoCsproj);
            var posicaoInclusao = conteudoCsproj.IndexOf("<Compile Include=", StringComparison.CurrentCulture);
            var inclusoesCsproj = nomesArquivos.Select(n => string.Format("<{0} Include=\"{1}\" />\n\t", tipo, n)).Aggregate((a, b) => a + b);
            File.WriteAllText(caminhoCompletoCsproj, conteudoCsproj.Insert(posicaoInclusao, inclusoesCsproj));
        }

        private static void ProcessaArquivo(string pathOrigemDentroTemplate, string pathDestinoCompleto)
        {
            const string pathTemplate = "../../Template";
            var arquivoModelo = File.ReadAllText(Path.Combine(pathTemplate, pathOrigemDentroTemplate));
            File.WriteAllText(pathDestinoCompleto, SubstituiTermos(arquivoModelo));
        }

        private static string SubstituiTermos(string conteudoArquivo)
        {
            conteudoArquivo = conteudoArquivo.Replace("Aviões", TituloPlural);
            conteudoArquivo = conteudoArquivo.Replace("Avião", TituloSingular);
            conteudoArquivo = conteudoArquivo.Replace("Avioes", ModeloPlural);
            conteudoArquivo = conteudoArquivo.Replace("Aviao", ModeloSingular);

            conteudoArquivo = conteudoArquivo.Replace("aviões", TransformaPrimeiroCaractereParaMinusculo(TituloPlural));
            conteudoArquivo = conteudoArquivo.Replace("avião", TransformaPrimeiroCaractereParaMinusculo(TituloSingular));
            conteudoArquivo = conteudoArquivo.Replace("avioes", TransformaPrimeiroCaractereParaMinusculo(ModeloPlural));
            conteudoArquivo = conteudoArquivo.Replace("aviao", TransformaPrimeiroCaractereParaMinusculo(ModeloSingular));

            conteudoArquivo = conteudoArquivo.Replace("%ARTIGO%", Artigo);

            return conteudoArquivo;
        }

        private static string TransformaPrimeiroCaractereParaMinusculo(string texto)
        {
            var primeiroCaractere = texto.Substring(0, 1);
            var restante = texto.Substring(1);
            return string.Format("{0}{1}", primeiroCaractere.ToLowerInvariant(), restante);
        }
    }
}
