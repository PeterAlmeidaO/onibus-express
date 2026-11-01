using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Tests.Fixtures;
using Xunit;

namespace OnibusExpress.Tests.Unit.Services
{
    public class ViagemServiceTests
    {
        private readonly Mock<IViagemRepository> _mockViagemRepository = new();
        private readonly Mock<IReservaRepository> _mockReservaRepository = new();
        private readonly ViagemService _service;

        public ViagemServiceTests()
        {
            _service = new ViagemService(_mockViagemRepository.Object, _mockReservaRepository.Object);
        }

        [Fact]
        public async Task DetalhesAsync_ComViagemInexistente_LancaNotFoundException()
        {
            _mockViagemRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Viagem?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DetalhesAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task DetalhesAsync_CalculaVagasRestantesPelosAssentosConfirmados()
        {
            var viagem = EntidadesFixture.CriarViagem(totalAssentos: 40);
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);
            _mockReservaRepository.Setup(r => r.GetAssentosConfirmadosAsync(viagem.Id)).ReturnsAsync(new[] { 1, 2, 3 });

            var resultado = await _service.DetalhesAsync(viagem.Id);

            Assert.Equal(new[] { 1, 2, 3 }, resultado.AssentosOcupados);
            Assert.Equal(37, resultado.VagasRestantes);
        }

        [Fact]
        public async Task BuscarAsync_MapeiaViagensParaResumo()
        {
            var viagem = EntidadesFixture.CriarViagem();
            _mockViagemRepository
                .Setup(r => r.BuscarAsync("São", null, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new[] { viagem });

            var resultado = (await _service.BuscarAsync("São", null, null)).Single();

            Assert.Equal(viagem.Id, resultado.Id);
            Assert.Equal("São Paulo", resultado.Origem);
            Assert.Equal("Rio de Janeiro", resultado.Destino);
        }
    }
}
