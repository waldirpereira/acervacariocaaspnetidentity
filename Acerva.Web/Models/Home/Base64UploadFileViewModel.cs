namespace Acerva.Web.Models.Home
{
    public class Base64UploadFileViewModel
    {
        public int Filesize { get; set; }
        public string Filetype { get; set; }
        public string Filename { get; set; }
        public string Base64 { get; set; }
    }
}