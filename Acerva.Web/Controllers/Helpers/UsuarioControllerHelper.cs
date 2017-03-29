using System;
using System.Drawing;
using System.IO;
using System.Web;

namespace Acerva.Web.Controllers.Helpers
{
    public class UsuarioControllerHelper
    {
        private const string CaminhoFotos = "~/Content/Aplicacao/images/fotos";
        private const string CaminhoImagens = "~/Content/Aplicacao/images";

        public void SalvaFoto(string userId, string fotoBase64, HttpContextBase httpContext)
        {
            var caminhoFotos = httpContext.Server.MapPath(CaminhoFotos);
            var caminhoCompleto = string.Format("{0}\\{1}.png", caminhoFotos, userId);

            if (fotoBase64 == null)
            {
                File.Delete(caminhoCompleto);
                return;
            }

            fotoBase64 = fotoBase64.Replace("data:image/png;base64,", "");
            var bytes = Convert.FromBase64String(fotoBase64);

            using (var imageFile = new FileStream(caminhoCompleto, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }

        public string BuscaFotoBase64(string userId, HttpContextBase httpContext, bool substituiPorPadraoSeNaoExistir = false)
        {
            var caminhoFotos = httpContext.Server.MapPath(CaminhoFotos);
            var caminhoCompleto = string.Format("{0}\\{1}.png", caminhoFotos, userId);

            if (!File.Exists(caminhoCompleto))
            {
                if (!substituiPorPadraoSeNaoExistir)
                    return null;

                caminhoCompleto = string.Format("{0}\\{1}", httpContext.Server.MapPath(CaminhoImagens), "tacas.png");
            }

            using (var image = Image.FromFile(caminhoCompleto))
            {
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    var imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    var base64String = Convert.ToBase64String(imageBytes);
                    return string.Format("data:image/png;base64,{0}", base64String);
                }
            }
        }
    }
}