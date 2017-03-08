using System;
using System.IO;

namespace CreateModel
{
    class Program
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

            // modelo
            var arquivoModelo = File.ReadAllText("./Template/Modelo/Aviao.cs");
            File.WriteAllText(string.Format("../Acerva.Modelo/{0}.cs", ModeloSingular), SubstituiTermos(arquivoModelo));

            var pathCsproj = "../Modelo/Acerva.Modelo.csproj";
            var modeloCsproj = File.ReadAllText(pathCsproj);
            var posicaoInclusao = modeloCsproj.IndexOf("<Compile Include=", StringComparison.CurrentCulture);
            File.WriteAllText(pathCsproj, modeloCsproj.Insert(posicaoInclusao, string.Format("<Compile Include=\"{0}.cs\" />\n\t", ModeloSingular)));

            // 

            Console.Write("Geração terminada com sucesso. Pressione ENTER tecla para finalizar.");
            Console.ReadLine();
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
