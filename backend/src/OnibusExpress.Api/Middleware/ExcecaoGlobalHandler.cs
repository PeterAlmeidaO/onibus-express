using Microsoft.AspNetCore.Diagnostics;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Domain.Exceptions;

namespace OnibusExpress.Api.Middleware
{
    // Converte exceções em respostas HTTP padronizadas { erro }; controllers ficam sem try/catch
    public class ExcecaoGlobalHandler : IExceptionHandler
    {
        private readonly ILogger<ExcecaoGlobalHandler> _logger;

        public ExcecaoGlobalHandler(ILogger<ExcecaoGlobalHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, mensagem) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                ConflitoException => (StatusCodes.Status409Conflict, exception.Message),
                RegraNegocioException or ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.")
            };

            // Detalhe de erro inesperado vai só para o log
            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new ErroDto(mensagem), cancellationToken);
            return true;
        }
    }
}
