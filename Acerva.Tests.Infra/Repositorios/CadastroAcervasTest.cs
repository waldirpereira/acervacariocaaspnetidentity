using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Infra.Repositorios
{
    [TestClass]
    public class CadastroAcervasTest : TesteComBancoDeDadosBase
    {
        private static ICadastroAcervas _repositorio;

        protected override void ExecutaAntesDeCadaTeste()
        {
            _repositorio = new CadastroAcervas(Session, new CadastroUsuarios(Session));
        }

        [TestMethod]
        public void DeveriaSalvarAcervaComNovoParticipante()
        {
            // given
            var acerva = _repositorio.Busca(5);
            acerva.Participacoes.Add(new Participacao {Codigo = 0, Usuario = new IdentityUser { Email = "horadochute@gmail.com", Name = "Hora"}, PontuacaoInicial = 0 });

            // when
            _repositorio.Salva(acerva);

            // then
        }
    }
}
