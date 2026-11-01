using System;

namespace OnibusExpress.Domain.Exceptions
{
    // Conflito de estado (ex.: assento já ocupado). Mapeada para HTTP 409.
    public class ConflitoException : Exception
    {
        public ConflitoException(string message) : base(message) { }
    }
}
