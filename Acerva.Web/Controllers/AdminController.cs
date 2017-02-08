using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using Acerva.Infra.Repositorios;
using Acerva.Infra.Web;
using Microsoft.Win32;

namespace Acerva.Web.Controllers
{
    [Authorize]
    [AcervaAuthorize(Roles = "ADMIN")]
    public class AdminController : ApplicationBaseController
    {
        public AdminController(ICadastroUsuarios cadastroUsuarios) : base(cadastroUsuarios)
        {
        }

        public ActionResult Index()
        {
            var arquivosLog = PegaListaArquivos(MvcApplication.PastaLog);
            
            var retorno = new Dictionary<string, IEnumerable<FileInfo>>();
            retorno.Add("Log", arquivosLog);
            
            return View(retorno);
        }

        public ActionResult AlteraHabilitacaoBundles(bool habilita)
        {
            BundleTable.EnableOptimizations = habilita;
            return Json(BundleTable.EnableOptimizations, JsonRequestBehavior.AllowGet);
        }
        
        private IEnumerable<FileInfo> PegaListaArquivos(string pasta)
        {
            IEnumerable<FileInfo> files;
            try
            {
                var dir = new DirectoryInfo(pasta);
                files = dir.GetFiles()
                    .OrderBy(t => t.Name);
            }
            catch (DirectoryNotFoundException)
            {
                files = new List<FileInfo>();
            }
            return files;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification =
                "O Stream não pode sofrer Dispose pois senão não ocorre o download. O FileResult se encarrega do Dispose."
            )]
        public ActionResult DownloadFile(string fileName, string folder)
        {
            var fileWithPath = Path.Combine(folder, fileName);
            var fs = new FileStream(fileWithPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return File(fs, "text/plain", fileName);
        }

        public ActionResult LimpaCacheDeSegundoNivelDoNHibernate()
        {
            MvcApplication.LimpaCacheDeSegundoNivelDaPersistencia();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscaVersaoAspNet()
        {
            return Json(Get45Or451FromRegistry(), JsonRequestBehavior.AllowGet);
        }

        private static string Get45Or451FromRegistry()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return CheckFor45DotVersion((int)ndpKey.GetValue("Release"));
                }

                return "Version 4.5 or later is not detected.";
            }
        }

        // Checking the version using >= will enable forward compatibility, 
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393295)
            {
                return "4.6 or later";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2 or later";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1 or later";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
}