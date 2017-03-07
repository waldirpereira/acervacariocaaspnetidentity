using System.Security.Principal;
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
     
            _controller = new RegionalController(_cadastroRegionais.Object, validator.Object, 
                user.Object, cadastroUsuarios.Object);
            _controller.SetFakeUrlHelper();
        }
        
    }
}
