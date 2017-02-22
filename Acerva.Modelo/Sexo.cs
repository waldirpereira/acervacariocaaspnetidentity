using System;
using Acerva.Utils;

namespace Acerva.Modelo
{
    [Serializable]
    public enum Sexo
    {
        NaoDefinido = 0,

        [CodigoBd("M")]
        [NomeExibicao("Masculino")]
        Masculino,

        [CodigoBd("F")]
        [NomeExibicao("Feminino")]
        Feminino,

        [CodigoBd("O")]
        [NomeExibicao("Outro")]
        Outro
    }
}
