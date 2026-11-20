using System;
using System.Threading.Tasks;
using Moq;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Tests.Fixtures;
using Xunit;

namespace OnibusExpress.Tests.Unit.Services
{
    public class ReservaServiceTests
    {
        private readonly Mock<IReservaRepository> _mockReservaRepository = new();
        private readonly Mock<IViagemRepository> _mockViagemRepository = new();
        private readonly Mock<IPassageiroRepository> _mockPassageiroRepository = new();
        private readonly ReservaService _service;

        public ReservaServiceTests()
        {
            _service = new ReservaService(_mockReservaRepository.Object, _mockViagemRepository.Object, _mockPassageiroRepository.Object);
        }

        private static CriarReservaDto CriarDto(Guid viagemId, int assento = 10) => new()
        {
            ViagemId = viagemId,
            Nome = "Maria Souza",
            Cpf = EntidadesFixture.CpfValido,
            Email = "maria@email.com",
            DataNascimento = new DateTime(1995, 5, 20),
            NumeroAssento = assento
        };

        [Fact]
        public async Task CriarReservaAsync_ComDadosValidos_GravaReservaERetornaCodigo()
        {
            // Arrange
            var viagem = EntidadesFixture.CriarViagem();
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);

            // Act
            var resultado = await _service.CriarReservaAsync(CriarDto(viagem.Id));

            // Assert
            Assert.StartsWith("ONI-", resultado.CodigoReserva);
            Assert.Equal("Confirmada", resultado.Status);
            _mockReservaRepository.Verify(r => r.AddAsync(It.Is<Reserva>(x => x.NumeroAssento == 10 && x.Passageiro.Nome == "Maria Souza")), Times.Once);
        }

        [Fact]
        public async Task CriarReservaAsync_ComViagemInexistente_LancaNotFoundException()
        {
            _mockViagemRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Viagem?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CriarReservaAsync(CriarDto(Guid.NewGuid())));
            _mockReservaRepository.Verify(r => r.AddAsync(It.IsAny<Reserva>()), Times.Never);
        }

        [Fact]
        public async Task CriarReservaAsync_ComViagemJaRealizada_LancaRegraNegocioException()
        {
            var viagem = EntidadesFixture.CriarViagem(DateTime.UtcNow.AddHours(-1));
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);

            await Assert.ThrowsAsync<RegraNegocioException>(() => _service.CriarReservaAsync(CriarDto(viagem.Id)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(41)]
        public async Task CriarReservaAsync_ComAssentoForaDoLimite_LancaArgumentException(int assento)
        {
            var viagem = EntidadesFixture.CriarViagem(totalAssentos: 40);
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarReservaAsync(CriarDto(viagem.Id, assento)));
        }

        [Fact]
        public async Task CriarReservaAsync_ComAssentoOcupado_LancaConflitoException()
        {
            var viagem = EntidadesFixture.CriarViagem();
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);
            _mockReservaRepository.Setup(r => r.ExisteAssentoConfirmadoAsync(viagem.Id, 10)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflitoException>(() => _service.CriarReservaAsync(CriarDto(viagem.Id)));
            _mockReservaRepository.Verify(r => r.AddAsync(It.IsAny<Reserva>()), Times.Never);
        }

        [Fact]
        public async Task CriarReservaAsync_ComPassageiroExistente_ReutilizaCadastroEAtualizaContato()
        {
            // Arrange
            var viagem = EntidadesFixture.CriarViagem();
            var existente = EntidadesFixture.CriarPassageiro("Nome Antigo", "antigo@email.com");
            _mockViagemRepository.Setup(r => r.GetByIdAsync(viagem.Id)).ReturnsAsync(viagem);
            _mockPassageiroRepository.Setup(r => r.GetByCpfAsync(EntidadesFixture.CpfValido)).ReturnsAsync(existente);

            // Act
            await _service.CriarReservaAsync(CriarDto(viagem.Id));

            // Assert
            Assert.Equal("Maria Souza", existente.Nome);
            Assert.Equal("maria@email.com", existente.Email);
            _mockReservaRepository.Verify(r => r.AddAsync(It.Is<Reserva>(x => x.PassageiroId == existente.Id)), Times.Once);
        }

        [Fact]
        public async Task ConsultarPorCodigoAsync_ComCodigoExistente_RetornaDetalheComCpfMascarado()
        {
            var reserva = EntidadesFixture.CriarReserva();
            _mockReservaRepository.Setup(r => r.GetByCodigoAsync(reserva.CodigoReserva)).ReturnsAsync(reserva);

            var resultado = await _service.ConsultarPorCodigoAsync(reserva.CodigoReserva);

            Assert.Equal(reserva.CodigoReserva, resultado.Codigo);
            Assert.Equal("***.982.247-**", resultado.Passageiro.Cpf);
        }

        [Fact]
        public async Task CancelarAsync_ComCodigoInexistente_LancaNotFoundException()
        {
            _mockReservaRepository.Setup(r => r.GetByCodigoAsync(It.IsAny<string>())).ReturnsAsync((Reserva?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelarAsync("ONI-INEXISTENTE"));
        }

        [Fact]
        public async Task CancelarAsync_DentroDoPrazo_AlteraStatusEGrava()
        {
            var reserva = EntidadesFixture.CriarReserva(DateTime.UtcNow.AddDays(1));
            _mockReservaRepository.Setup(r => r.GetByCodigoAsync(reserva.CodigoReserva)).ReturnsAsync(reserva);

            await _service.CancelarAsync(reserva.CodigoReserva);

            Assert.Equal(ReservaStatus.Cancelada, reserva.Status);
            _mockReservaRepository.Verify(r => r.UpdateAsync(reserva), Times.Once);
        }

        [Fact]
        public async Task CancelarAsync_ForaDoPrazo_NaoGrava()
        {
            var reserva = EntidadesFixture.CriarReserva(DateTime.UtcNow.AddHours(1));
            _mockReservaRepository.Setup(r => r.GetByCodigoAsync(reserva.CodigoReserva)).ReturnsAsync(reserva);

            await Assert.ThrowsAsync<RegraNegocioException>(() => _service.CancelarAsync(reserva.CodigoReserva));
            _mockReservaRepository.Verify(r => r.UpdateAsync(It.IsAny<Reserva>()), Times.Never);
        }
    }
}
