using System;
using SiplexCommon.EnumUtil;

namespace Acerva.Modelo
{
    [Serializable]
    public enum StatusUsuario
    {
        NaoDefinido = 0,

        [CodigoBd("E")]
        [NomeExibicao("Ag. confirmação e-mail")]
        AguardandoConfirmacaoEmail,

        [CodigoBd("C")]
        [NomeExibicao("Cancelado")]
        Cancelado,

        [CodigoBd("A")]
        [NomeExibicao("Ativo")]
        Ativo,

        [CodigoBd("N")]
        [NomeExibicao("Novo")]
        Novo,

        [CodigoBd("I")]
        [NomeExibicao("Ag. indicação")]
        AguardandoIndicacao,

        [CodigoBd("P")]
        [NomeExibicao("Ag. pagto. anuidade")]
        AguardandoPagamentoAnuidade
    }
}
