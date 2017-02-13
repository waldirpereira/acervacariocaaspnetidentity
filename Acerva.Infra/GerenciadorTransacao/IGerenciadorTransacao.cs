using System;

namespace Acerva.Infra.GerenciadorTransacao
{
    public interface IGerenciadorTransacao
    {
        void RegistraPostCommit(Action f);
    }
}