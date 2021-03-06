﻿using System;
using Acerva.Utils;

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
        [NomeExibicao("Inativado")]
        Inativo,

        [CodigoBd("A")]
        [NomeExibicao("Ativo")]
        Ativo,

        [CodigoBd("N")]
        [NomeExibicao("Novo")]
        Novo,

        [CodigoBd("I")]
        [NomeExibicao("Ag. indicação")]
        AguardandoIndicacao,

        [CodigoBd("R")]
        [NomeExibicao("Ag. renovação")]
        AguardandoRenovacao,

        [CodigoBd("P")]
        [NomeExibicao("Ag. pagto. anuidade")]
        AguardandoPagamentoAnuidade
    }
}
