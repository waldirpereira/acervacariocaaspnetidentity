using System.Security.Principal;
using AutoMapper;
using Acerva.Infra.Repositorios;
using Acerva.Web.Controllers;
using Acerva.Web.Models.CadastroAcervas;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Acerva.Tests.Web.Controllers
{
    [TestClass]
    public class AcervaManagerControllerTest
    {
        private MinhaAcervaController _controller;

        public AcervaManagerControllerTest()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CadastroAcervaMapperProfile>();
            });
        }

        [TestInitialize]
        public void SetUp()
        {
            var cadastroAcervas = new Mock<ICadastroAcervas>();
            var validator = new Mock<IValidator<Modelo.Acerva>>();
            var user = new Mock<IPrincipal>();
            var cadastroUsuarios = new Mock<ICadastroUsuarios>();
            var cadastroParticipacoes = new Mock<ICadastroParticipacoes>();
            _controller = new MinhaAcervaController(cadastroAcervas.Object, validator.Object, user.Object, cadastroUsuarios.Object, cadastroParticipacoes.Object);
        }

        [TestMethod]
        public void SalvaDeveriaSalvarNovoParticipante()
        {
            // given
            var acervaViewModel = new Mock<AcervaViewModel>();
            
            // when

            // then
        }
    }
}
