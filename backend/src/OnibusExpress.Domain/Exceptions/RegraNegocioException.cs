using System;

namespace OnibusExpress.Domain.Exceptions
{
    // Violação de regra de negócio (ex.: prazo de cancelamento). Mapeada para HTTP 400.
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string message) : base(message) { }
    }
}
