using System;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.ValueObjects;

namespace OnibusExpress.Tests.Fixtures
{
    public static class EntidadesFixture
    {
        public const string CpfValido = "52998224725";

        public static Viagem CriarViagem(DateTime? partida = null, int totalAssentos = 40) =>
            new(new Rota("São Paulo", "Rio de Janeiro", TimeSpan.FromHours(6)), partida ?? DateTime.UtcNow.AddDays(1), 150m, totalAssentos);

        public static Passageiro CriarPassageiro(string nome = "João Silva", string email = "joao@email.com") =>
            new(nome, new Cpf(CpfValido), email, new DateTime(1990, 1, 1));

        public static Reserva CriarReserva(DateTime? partida = null) =>
            new(CriarViagem(partida), CriarPassageiro(), 10);
    }
}
