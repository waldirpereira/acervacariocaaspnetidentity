using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CreateModel
{
    public class Program
    {
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

            const string prefixoSistema = "Acerva";

            // modelo
            const string pathModelo = "../../../Acerva.Modelo";
            ProcessaArquivo("Modelo/Aviao.cs", Path.Combine(pathModelo, string.Format("{0}.cs", ModeloSingular)));
            ProcessaArquivo("Modelo/Validadores/AviaoValidator.cs", Path.Combine(pathModelo, string.Format("Validadores/{0}Validator.cs", ModeloSingular)));
            AlteraCsproj(Path.Combine(pathModelo, string.Format("{0}.Modelo.csproj", prefixoSistema)), new List<string> {
                    string.Format("{0}.cs", ModeloSingular),
                    string.Format("Validadores\\{0}Validator.cs", ModeloSingular)
                });
            
            // mapeamento


            Console.Write("Geração terminada com sucesso. Pressione ENTER tecla para finalizar.");
            Console.ReadLine();
        }

        private static void AlteraCsproj(string caminhoCompletoCsproj, IEnumerable<string> nomesArquivos)
        {
            var modeloCsproj = File.ReadAllText(caminhoCompletoCsproj);
            var posicaoInclusao = modeloCsproj.IndexOf("<Compile Include=", StringComparison.CurrentCulture);
            var inclusoesCsprojModelo = nomesArquivos.Select(n => string.Format("<Compile Include=\"{0}\" />\n\t", n)).Aggregate((a, b) => a + b);
            File.WriteAllText(caminhoCompletoCsproj, modeloCsproj.Insert(posicaoInclusao, inclusoesCsprojModelo));
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
