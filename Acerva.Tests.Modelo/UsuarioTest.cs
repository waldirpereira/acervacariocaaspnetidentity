using Acerva.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Modelo
{
    [TestClass]
    public class UsuarioTest
    {
        [TestMethod]
        public void EstaAssociadoDeveriaRetrnarTrueSeStatusEhAtivo()
        {
            // given
            var usuario = new Usuario { Status = StatusUsuario.Ativo };

            // when


            // then
            Assert.IsTrue(usuario.EstaAssociado);
        }

        [TestMethod]
        public void EstaAssociadoDeveriaRetrnarTrueSeStatusEhAguardandoRenovacao()
        {
            // given
            var usuario = new Usuario { Status = StatusUsuario.AguardandoRenovacao };

            // when


            // then
            Assert.IsTrue(usuario.EstaAssociado);
        }

        [TestMethod]
        public void EstaAssociadoDeveriaRetrnarTrueSeStatusNaoEhAtivoNemAguardandoRenovacao()
        {
            // given
            var usuario = new Usuario { Status = StatusUsuario.AguardandoPagamentoAnuidade };

            // when


            // then
            Assert.IsFalse(usuario.EstaAssociado);
        }
    }
}
