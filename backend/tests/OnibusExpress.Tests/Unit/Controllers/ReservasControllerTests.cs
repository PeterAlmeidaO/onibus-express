using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnibusExpress.Api.Controllers;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using Xunit;

namespace OnibusExpress.Tests.Unit.Controllers
{
    public class ReservasControllerTests
    {
        private readonly Mock<IReservaService> _mockService = new();
        private readonly ReservasController _controller;

        public ReservasControllerTests()
        {
            _controller = new ReservasController(_mockService.Object);
        }

        [Fact]
        public async Task Criar_RetornaCreatedComLocalizacaoDaConsulta()
        {
            var criada = new ReservaCriadaDto("ONI-ABCDEF1234", "Confirmada");
            _mockService.Setup(s => s.CriarReservaAsync(It.IsAny<CriarReservaDto>())).ReturnsAsync(criada);

            var resultado = await _controller.Criar(new CriarReservaDto());

            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(nameof(ReservasController.ConsultarPorCodigo), created.ActionName);
            Assert.Equal("ONI-ABCDEF1234", created.RouteValues!["codigo"]);
            Assert.Same(criada, created.Value);
        }

        [Fact]
        public async Task ConsultarPorCodigo_RetornaOkComDetalhe()
        {
            var detalhe = new ReservaDetalheDto("ONI-ABCDEF1234", "Confirmada", 10,
                new ReservaViagemDto("São Paulo", "Rio de Janeiro", DateTime.UtcNow), new ReservaPassageiroDto("João", "***.982.247-**"));
            _mockService.Setup(s => s.ConsultarPorCodigoAsync("ONI-ABCDEF1234", "52998224725")).ReturnsAsync(detalhe);

            var resultado = await _controller.ConsultarPorCodigo("ONI-ABCDEF1234", "52998224725");

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(detalhe, ok.Value);
        }

        [Fact]
        public async Task CancelarReserva_RetornaNoContent()
        {
            var resultado = await _controller.CancelarReserva("ONI-ABCDEF1234", "52998224725");

            Assert.IsType<NoContentResult>(resultado);
            _mockService.Verify(s => s.CancelarAsync("ONI-ABCDEF1234", "52998224725"), Times.Once);
        }
    }
}
