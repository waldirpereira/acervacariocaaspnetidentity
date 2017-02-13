using Acerva.Modelo;

namespace Acerva.Infra.Repositorios
{
    public interface ICadastroHistoricoStatusUsuarios
    {
        void Salva(HistoricoStatusUsuario historico);
    }
}