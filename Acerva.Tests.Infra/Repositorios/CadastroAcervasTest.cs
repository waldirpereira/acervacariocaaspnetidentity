using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Infra.Repositorios
{
    [TestClass]
    public class CadastroRegionaisTest : TesteComBancoDeDadosBase
    {
        private static ICadastroRegionais _repositorio;

        protected override void ExecutaAntesDeCadaTeste()
        {
            _repositorio = new CadastroRegionais(Session);
        }

        //[TestMethod]
        //public void DeveriaSalvarRegional()
        //{
        //    // given
        //    var regional = _repositorio.Busca(1);
            
        //    // when
        //    _repositorio.Salva(regional);

        //    // then
        //}
    }
}
