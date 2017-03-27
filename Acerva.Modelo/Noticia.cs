using System;
using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class Noticia
    {
        public const int TamanhoMaximoTitulo = 200;
        public const int TamanhoMaximoTextoHtml = 2000;

        public virtual int Codigo { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string TextoHtml { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual int? Ordem { get; set; }
        public virtual bool MostraListaAnexos { get; set; }

        private IEnumerable<AnexoNoticia> _anexos = new List<AnexoNoticia>();
        public virtual IEnumerable<AnexoNoticia> Anexos
        {
            get { return _anexos; }
            set { _anexos = value; }
        }
    }
}
