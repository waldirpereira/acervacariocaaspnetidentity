using System;
using SiplexCommon.EnumUtil;

namespace Acerva.Modelo
{
    [Serializable]
    public enum StatusUsuario
    {
        NaoDefinido = 0,

        [CodigoBd("I")]
        [NomeExibicao("Inativo")]
        Inativo,

        [CodigoBd("A")]
        [NomeExibicao("Ativo")]
        Ativo,

        [CodigoBd("N")]
        [NomeExibicao("Novo")]
        Novo,

        [CodigoBd("G")]
        [NomeExibicao("Ag. indicação")]
        AguardandoIndicacao,

        [CodigoBd("P")]
        [NomeExibicao("Ag. anuidade")]
        AguardandoPagamentoAnuidade
    }
}
