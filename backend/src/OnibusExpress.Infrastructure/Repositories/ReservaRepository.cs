using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Infrastructure.Context;

namespace OnibusExpress.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExisteAssentoConfirmadoAsync(Guid viagemId, int numeroAssento) =>
            _context.Reservas.AnyAsync(r => r.ViagemId == viagemId && r.NumeroAssento == numeroAssento && r.Status == ReservaStatus.Confirmada);

        public async Task AddAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg
                                               && pg.ConstraintName == AppDbContext.IndiceAssentoUnico)
            {
                // Outra requisição reservou o mesmo assento entre a checagem e a gravação
                throw new InvalidOperationException("Este assento já está ocupado.");
            }
        }
    }
}
