using System;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.ValueObjects;
using Xunit;

namespace OnibusExpress.Tests
{
    public class PassageiroTests
    {
        [Fact]
        public void Deve_Normalizar_Data_De_Nascimento_Para_Utc_Sem_Hora()
        {
            // Arrange: data como chega do JSON "1990-01-01T15:30:00" (Kind Unspecified)
            var dataSemFuso = new DateTime(1990, 1, 1, 15, 30, 0, DateTimeKind.Unspecified);

            // Act
            var passageiro = new Passageiro("João Silva", new Cpf("52998224725"), "joao@email.com", dataSemFuso);

            // Assert
            Assert.Equal(DateTimeKind.Utc, passageiro.DataNascimento.Kind);
            Assert.Equal(new DateTime(1990, 1, 1), passageiro.DataNascimento.Date);
            Assert.Equal(TimeSpan.Zero, passageiro.DataNascimento.TimeOfDay);
        }
    }
}
