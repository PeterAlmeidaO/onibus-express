using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;
using Xunit;

namespace OnibusExpress.Tests.Unit.Services
{
    public class RotaServiceTests
    {
        [Fact]
        public async Task ListarTodasAsync_MapeiaRotasParaDto()
        {
            // Arrange
            var rota = new Rota("Guarulhos", "Campinas", TimeSpan.FromHours(2));
            var mockRepository = new Mock<IRotaRepository>();
            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { rota });

            // Act
            var resultado = (await new RotaService(mockRepository.Object).ListarTodasAsync()).Single();

            // Assert
            Assert.Equal(rota.Id, resultado.Id);
            Assert.Equal(TimeSpan.FromHours(2), resultado.DuracaoEstimada);
        }
    }
}
