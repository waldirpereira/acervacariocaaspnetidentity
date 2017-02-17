using System;
using Acerva.Utils;

namespace Acerva.Modelo
{
    [Serializable]
    public enum VisibilidadeArtigo
    {
        NaoDefinido = 0,

        [CodigoBd("P")]
        [NomeExibicao("Público")]
        Publico,

        [CodigoBd("U")]
        [NomeExibicao("Autenticado")]
        Autenticado,

        [CodigoBd("I")]
        [NomeExibicao("Interno do sistema")]
        InternoSistema
    }
}
