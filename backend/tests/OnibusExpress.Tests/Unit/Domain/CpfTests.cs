using System;
using OnibusExpress.Domain.ValueObjects;
using Xunit;

namespace OnibusExpress.Tests
{
    public class CpfTests
    {
        [Theory]
        [InlineData("00000000000")] // Todos os dígitos iguais
        [InlineData("12345678901")] // Dígitos verificadores incorretos
        [InlineData("abc")]         // Letras
        [InlineData("")]            // Vazio
        public void Nao_Deve_Criar_Cpf_Invalido(string cpfInvalido)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Cpf(cpfInvalido));
        }

        [Fact]
        public void Deve_Criar_Cpf_Com_Numeracao_Valida()
        {
            // Arrange
            var cpfValido = "52998224725"; // Exemplo de CPF com cálculo válido

            // Act
            var cpf = new Cpf(cpfValido);

            // Assert
            Assert.Equal(cpfValido, cpf.Numero);
        }

        [Fact]
        public void Deve_Mascarar_Cpf_Exibindo_Apenas_Digitos_Centrais()
        {
            var cpf = new Cpf("529.982.247-25");

            Assert.Equal("***.982.247-**", cpf.Mascarado());
        }
    }
}