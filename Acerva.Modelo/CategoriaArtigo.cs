using System.Collections.Generic;

namespace Acerva.Modelo
{
    public class CategoriaArtigo
    {
        public const int TamanhoMaximoNome = 60;
        
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }

        private IEnumerable<Artigo> _artigos = new List<Artigo>();

        public virtual IEnumerable<Artigo> Artigos
        {
            get { return _artigos; }
            set { _artigos = value; }
        }
    }
}
