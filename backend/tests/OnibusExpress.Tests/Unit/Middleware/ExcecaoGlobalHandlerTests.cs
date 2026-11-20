using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using OnibusExpress.Api.Middleware;
using OnibusExpress.Domain.Exceptions;
using Xunit;

namespace OnibusExpress.Tests.Unit.Middleware
{
    public class ExcecaoGlobalHandlerTests
    {
        private static async Task<(int Status, string Corpo)> Executar(Exception excecao)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var handler = new ExcecaoGlobalHandler(NullLogger<ExcecaoGlobalHandler>.Instance);

            await handler.TryHandleAsync(context, excecao, CancellationToken.None);

            context.Response.Body.Position = 0;
            return (context.Response.StatusCode, await new StreamReader(context.Response.Body).ReadToEndAsync());
        }

        [Theory]
        [InlineData(typeof(NotFoundException), 404)]
        [InlineData(typeof(ConflitoException), 409)]
        [InlineData(typeof(RegraNegocioException), 400)]
        [InlineData(typeof(ArgumentException), 400)]
        public async Task TryHandleAsync_MapeiaExcecaoConhecidaParaStatusComMensagem(Type tipo, int statusEsperado)
        {
            var (status, corpo) = await Executar((Exception)Activator.CreateInstance(tipo, "mensagem de teste")!);

            Assert.Equal(statusEsperado, status);
            Assert.Contains("mensagem de teste", corpo);
        }

        [Fact]
        public async Task TryHandleAsync_ExcecaoInesperada_Retorna500SemDetalhesInternos()
        {
            var (status, corpo) = await Executar(new InvalidOperationException("connection string secreta"));

            Assert.Equal(500, status);
            Assert.DoesNotContain("secreta", corpo);
        }
    }
}
