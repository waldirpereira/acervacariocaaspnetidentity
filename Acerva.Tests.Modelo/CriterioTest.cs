using System.Collections.Generic;
using Acerva.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Modelo
{
    [TestClass]
    public class CriterioTest : EqualsHashCodeContract<Criterio>
    {
        protected override Criterio CreateInstance()
        {
            return new Criterio { Codigo = 1 };
        }

        protected override IEnumerable<Criterio> CreateNotEqualInstances()
        {
            return new List<Criterio> { new Criterio { Codigo = 2 }, new Criterio { Codigo = 3 } };
        }

        [TestMethod]
        public void SatisfazCriterioPlacarCheioTest()
        {
            // given
            var partida = new Partida {PlacarMandante = 1, PlacarVisitante = 0};

            var palpiteCheio = new Palpite {PlacarMandante = 1, PlacarVisitante = 0};

            var palpiteDif1 = new Palpite {PlacarMandante = 1, PlacarVisitante = 1};
            var palpiteDif2 = new Palpite {PlacarMandante = 0, PlacarVisitante = 1};
            var palpiteDif3 = new Palpite {PlacarMandante = 0, PlacarVisitante = 0};
            var palpiteDif4 = new Palpite {PlacarMandante = 2, PlacarVisitante = 1};
            var palpiteDif5 = new Palpite {PlacarMandante = 1, PlacarVisitante = 2};

            // when
            
            // then
            Assert.IsTrue(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteCheio));

            Assert.IsFalse(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteDif1));
            Assert.IsFalse(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteDif2));
            Assert.IsFalse(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteDif3));
            Assert.IsFalse(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteDif4));
            Assert.IsFalse(Criterio.CriterioPlacarCheio.Satisfaz(partida, palpiteDif5));
        }

        [TestMethod]
        public void SatisfazCriterioVencedorESaldoTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 1, PlacarVisitante = 0 };

            var palpiteCheio = new Palpite { PlacarMandante = 1, PlacarVisitante = 0 };
            var palpiteSaldoIgual = new Palpite { PlacarMandante = 2, PlacarVisitante = 1 };

            var palpiteSaldoDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 1 };
            var palpiteSaldoDif2 = new Palpite { PlacarMandante = 0, PlacarVisitante = 0 };
            var palpiteSaldoDif3 = new Palpite { PlacarMandante = 3, PlacarVisitante = 1 };
            var palpiteSaldoDif4 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };

            // when

            // then
            Assert.IsTrue(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteCheio));
            Assert.IsTrue(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteSaldoIgual));

            Assert.IsFalse(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteSaldoDif1));
            Assert.IsFalse(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteSaldoDif2));
            Assert.IsFalse(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteSaldoDif3));
            Assert.IsFalse(Criterio.CriterioVencedorESaldo.Satisfaz(partida, palpiteSaldoDif4));
        }

        [TestMethod]
        public void SatisfazCriterioVencedorEPlacarVencedorTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 2, PlacarVisitante = 0 };

            var palpiteCheio = new Palpite { PlacarMandante = 2, PlacarVisitante = 0 };
            var palpitePlacarVencedor = new Palpite { PlacarMandante = 2, PlacarVisitante = 1 };

            var palpiteDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 2 };
            var palpiteDif2 = new Palpite { PlacarMandante = 0, PlacarVisitante = 0 };
            var palpiteDif3 = new Palpite { PlacarMandante = 3, PlacarVisitante = 1 };
            var palpiteDif4 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };
            var palpiteDif5 = new Palpite { PlacarMandante = 2, PlacarVisitante = 2 };

            // when

            // then
            Assert.IsTrue(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteCheio));
            Assert.IsTrue(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpitePlacarVencedor));

            Assert.IsFalse(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteDif1));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteDif2));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteDif3));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteDif4));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarVencedor.Satisfaz(partida, palpiteDif5));
        }

        [TestMethod]
        public void SatisfazCriterioVencedorEPlacarPerdedorTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 2, PlacarVisitante = 0 };

            var palpiteCheio = new Palpite { PlacarMandante = 2, PlacarVisitante = 0 };
            var palpitePlacarPerdedor = new Palpite { PlacarMandante = 1, PlacarVisitante = 0 };

            var palpiteDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 2 };
            var palpiteDif2 = new Palpite { PlacarMandante = 0, PlacarVisitante = 0 };
            var palpiteDif3 = new Palpite { PlacarMandante = 3, PlacarVisitante = 1 };
            var palpiteDif4 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };
            var palpiteDif5 = new Palpite { PlacarMandante = 2, PlacarVisitante = 2 };

            // when

            // then
            Assert.IsTrue(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteCheio));
            Assert.IsTrue(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpitePlacarPerdedor));

            Assert.IsFalse(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteDif1));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteDif2));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteDif3));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteDif4));
            Assert.IsFalse(Criterio.CriterioVencedorEPlacarPerdedor.Satisfaz(partida, palpiteDif5));
        }

        [TestMethod]
        public void SatisfazCriterioVencedorTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 2, PlacarVisitante = 0 };

            var palpiteCheio = new Palpite { PlacarMandante = 2, PlacarVisitante = 0 };
            var palpiteVencedor = new Palpite { PlacarMandante = 5, PlacarVisitante = 1 };
            var palpiteVencedorESaldo = new Palpite { PlacarMandante = 4, PlacarVisitante = 2 };
            var palpitePlacarVencedor = new Palpite { PlacarMandante = 2, PlacarVisitante = 1 };
            var palpitePlacarPerdedor = new Palpite { PlacarMandante = 1, PlacarVisitante = 0 };

            var palpiteDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 2 };
            var palpiteDif2 = new Palpite { PlacarMandante = 0, PlacarVisitante = 0 };
            var palpiteDif3 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };
            var palpiteDif4 = new Palpite { PlacarMandante = 2, PlacarVisitante = 2 };

            // when

            // then
            Assert.IsTrue(Criterio.CriterioVencedor.Satisfaz(partida, palpiteCheio));
            Assert.IsTrue(Criterio.CriterioVencedor.Satisfaz(partida, palpiteVencedor));
            Assert.IsTrue(Criterio.CriterioVencedor.Satisfaz(partida, palpiteVencedorESaldo));
            Assert.IsTrue(Criterio.CriterioVencedor.Satisfaz(partida, palpitePlacarVencedor));
            Assert.IsTrue(Criterio.CriterioVencedor.Satisfaz(partida, palpitePlacarPerdedor));

            Assert.IsFalse(Criterio.CriterioVencedor.Satisfaz(partida, palpiteDif1));
            Assert.IsFalse(Criterio.CriterioVencedor.Satisfaz(partida, palpiteDif2));
            Assert.IsFalse(Criterio.CriterioVencedor.Satisfaz(partida, palpiteDif3));
            Assert.IsFalse(Criterio.CriterioVencedor.Satisfaz(partida, palpiteDif4));
        }

        [TestMethod]
        public void SatisfazCriterioEmpateTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 2, PlacarVisitante = 2 };

            var palpiteCheio = new Palpite { PlacarMandante = 2, PlacarVisitante = 2 };
            var palpiteEmpateDiferente = new Palpite { PlacarMandante = 5, PlacarVisitante = 5 };
            
            var palpiteDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 2 };
            var palpiteDif2 = new Palpite { PlacarMandante = 1, PlacarVisitante = 0 };
            var palpiteDif3 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };
            var palpiteDif4 = new Palpite { PlacarMandante = 2, PlacarVisitante = 1 };

            // when

            // then
            Assert.IsTrue(Criterio.CriterioEmpate.Satisfaz(partida, palpiteCheio));
            Assert.IsTrue(Criterio.CriterioEmpate.Satisfaz(partida, palpiteEmpateDiferente));

            Assert.IsFalse(Criterio.CriterioEmpate.Satisfaz(partida, palpiteDif1));
            Assert.IsFalse(Criterio.CriterioEmpate.Satisfaz(partida, palpiteDif2));
            Assert.IsFalse(Criterio.CriterioEmpate.Satisfaz(partida, palpiteDif3));
            Assert.IsFalse(Criterio.CriterioEmpate.Satisfaz(partida, palpiteDif4));
        }

        [TestMethod]
        public void SatisfazCriterioNenhumAcertoTest()
        {
            // given
            var partida = new Partida { PlacarMandante = 2, PlacarVisitante = 1 };

            var palpiteCheio = new Palpite { PlacarMandante = 2, PlacarVisitante = 1 };
            var palpiteSaldo = new Palpite { PlacarMandante = 1, PlacarVisitante = 0 };
            var palpitePlacarPerdedor = new Palpite { PlacarMandante = 3, PlacarVisitante = 1 };
            var palpitePlacarVencedor = new Palpite { PlacarMandante = 2, PlacarVisitante = 0 };
            var palpiteVencedor = new Palpite { PlacarMandante = 5, PlacarVisitante = 4 };
            var palpitePlacarUmaDasEquipesSendoElaAVencedora = new Palpite { PlacarMandante = 2, PlacarVisitante = 2 };
            var palpitePlacarUmaDasEquipesSendoElaAPerdedora = new Palpite { PlacarMandante = 1, PlacarVisitante = 1 };

            var palpiteDif1 = new Palpite { PlacarMandante = 0, PlacarVisitante = 2 };
            var palpiteDif2 = new Palpite { PlacarMandante = 0, PlacarVisitante = 0 };
            var palpiteDif3 = new Palpite { PlacarMandante = 1, PlacarVisitante = 2 };
            var palpiteDif4 = new Palpite { PlacarMandante = 3, PlacarVisitante = 3 };

            // when

            // then
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteCheio));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteSaldo));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpitePlacarPerdedor));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpitePlacarVencedor));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteVencedor));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpitePlacarUmaDasEquipesSendoElaAVencedora));
            Assert.IsFalse(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpitePlacarUmaDasEquipesSendoElaAPerdedora));

            Assert.IsTrue(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteDif1));
            Assert.IsTrue(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteDif2));
            Assert.IsTrue(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteDif3));
            Assert.IsTrue(Criterio.CriterioNenhumAcerto.Satisfaz(partida, palpiteDif4));
        }
    }
}
