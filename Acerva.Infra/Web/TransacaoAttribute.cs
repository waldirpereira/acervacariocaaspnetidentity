using System;

namespace Acerva.Infra.Web
{
    /// <summary>
    /// Marca uma action ou controller para que ele use o TransacaoFilter. 
    /// Esta associação entre o filter e o attribute é feita pelo container de injeção de dependência
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class TransacaoAttribute : Attribute
    {

    }
}
