using System.Web.Mvc;
using Acerva.Infra.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Acerva.Web.Controllers;
using Moq;

namespace Acerva.Tests.Web.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private Mock<ICadastroUsuarios> _cadastroUsuarios;
        private HomeController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _cadastroUsuarios = new Mock<ICadastroUsuarios>();
            _controller = new HomeController(_cadastroUsuarios.Object);
            _controller.SetFakeUrlHelper();
        }

        [TestMethod]
        public void Index()
        {
            // Arrange

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange

            // Act
            var result = _controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange

            // Act
            var result = _controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
