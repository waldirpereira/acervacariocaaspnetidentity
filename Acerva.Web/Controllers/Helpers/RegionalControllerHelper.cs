using Acerva.Infra.Repositorios;

namespace Acerva.Web.Controllers.Helpers
{
    public class RegionalControllerHelper
    {
        private readonly ICadastroRegionais _cadastroRegionais;

        public RegionalControllerHelper(ICadastroRegionais cadastroRegionais)
        {
            _cadastroRegionais = cadastroRegionais;
        }
    }
}