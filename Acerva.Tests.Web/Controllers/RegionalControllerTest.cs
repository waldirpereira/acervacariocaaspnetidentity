using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Acerva.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Acerva.Web.Controllers;
using FluentValidation;
using Moq;

namespace Acerva.Tests.Web.Controllers
{
    [TestClass]
    public class RegionalControllerTest
    {
        private RegionalController _controller;
        Mock<ICadastroRegionais> _cadastroRegionais;

        [TestInitialize]
        public void SetUp()
        {
            
            _cadastroRegionais = new Mock<ICadastroRegionais>();
            var validator = new Mock<IValidator<Regional>>();
            var user = new Mock<IPrincipal>();
            var cadastroUsuarios = new Mock<ICadastroUsuarios>();
     
            _controller = new RegionalController(_cadastroRegionais.Object, validator.Object, user.Object, cadastroUsuarios.Object);
            _controller.SetFakeUrlHelper();
        }

        [TestMethod]
        public void BuscaPlacares()
        {
            // given
            var flamengo = new Equipe { Codigo = 1, Nome = "Flamengo" };
            var botafogo = new Equipe { Codigo = 2, Nome = "Botafogo" };

            var rodada = new Rodada
            {
                Partidas = new List<Partida>
                {
                    new Partida { EquipeMandante = botafogo, EquipeVisitante = flamengo, Terminada = false }
                }
            };
            var equipes = new List<Equipe>
            {
                flamengo,
                botafogo,
            };
            _cadastroRegionais.Setup(c => c.BuscaRodada(15)).Returns(rodada);
            _cadastroRegionais.Setup(c => c.BuscaEquipes()).Returns(equipes);

            // when
            var result = _controller.BuscaPlacares(15) as ViewResult;

            // then
            Assert.IsNotNull(result);
        }
    }
}
