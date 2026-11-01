using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.ValueObjects;
using OnibusExpress.Infrastructure.Context;

namespace OnibusExpress.Application.Services
{
    public class ReservaApplicationService
    {
        private readonly AppDbContext _context;

        public ReservaApplicationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reserva> CriarReservaAsync(Guid viagemId, string nome, string cpfInput, string email, DateTime dataNascimento, int numeroAssento)
        {
            var cpf = new Cpf(cpfInput);

            var viagem = await _context.Viagens.FindAsync(viagemId);
            if (viagem == null) throw new NotFoundException("Viagem não encontrada.");

            if (viagem.IsPassada()) 
                throw new InvalidOperationException("Não é possível reservar passagens para uma viagem que já ocorreu.");

            if (numeroAssento <= 0 || numeroAssento > viagem.TotalAssentos)
                throw new ArgumentException("Número de assento inválido para esta viagem.");

            var assentoOcupado = await _context.Reservas
                .AnyAsync(r => r.ViagemId == viagemId && r.NumeroAssento == numeroAssento && r.Status == ReservaStatus.Confirmada);

            if (assentoOcupado)
                throw new InvalidOperationException("Este assento já está ocupado.");

            var passageiro = await _context.Passageiros
                .FirstOrDefaultAsync(p => p.Cpf.Numero == cpf.Numero);

            if (passageiro == null)
            {
                passageiro = new Passageiro(nome, cpf, email, dataNascimento);
                _context.Passageiros.Add(passageiro);
            }
            else
            {
                // Passageiro já cadastrado pelo CPF: mantém o cadastro e atualiza os dados de contato informados
                passageiro.AtualizarContato(nome, email);
            }

            var reserva = new Reserva(viagem, passageiro, numeroAssento);

            _context.Reservas.Add(reserva);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg
                                               && pg.ConstraintName == AppDbContext.IndiceAssentoUnico)
            {
                // Outra requisição reservou o mesmo assento entre a checagem acima e a gravação
                throw new InvalidOperationException("Este assento já está ocupado.");
            }

            return reserva;
        }
    }
}