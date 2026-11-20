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

        [Fact]
        public void Deve_Atualizar_Nome_E_Email_Do_Passageiro()
        {
            var passageiro = new Passageiro("João Silva", new Cpf("52998224725"), "joao@email.com", new DateTime(1990, 1, 1));

            passageiro.AtualizarContato("João S. Silva", "novo@email.com");

            Assert.Equal("João S. Silva", passageiro.Nome);
            Assert.Equal("novo@email.com", passageiro.Email);
        }

        [Theory]
        [InlineData("", "novo@email.com")]
        [InlineData("João", " ")]
        public void Nao_Deve_Atualizar_Contato_Com_Dados_Vazios(string nome, string email)
        {
            var passageiro = new Passageiro("João Silva", new Cpf("52998224725"), "joao@email.com", new DateTime(1990, 1, 1));

            Assert.Throws<ArgumentException>(() => passageiro.AtualizarContato(nome, email));
        }
    }
}
