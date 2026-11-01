using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
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

        public async Task<IEnumerable<int>> GetAssentosConfirmadosAsync(Guid viagemId) =>
            await _context.Reservas
                .Where(r => r.ViagemId == viagemId && r.Status == ReservaStatus.Confirmada)
                .Select(r => r.NumeroAssento)
                .ToListAsync();

        // Rastreada: o cancelamento altera o status e é gravado via UpdateAsync
        public async Task<Reserva?> GetByCodigoAsync(string codigo) =>
            await _context.Reservas
                .Include(r => r.Viagem).ThenInclude(v => v.Rota)
                .Include(r => r.Passageiro)
                .FirstOrDefaultAsync(r => r.CodigoReserva == codigo);

        public Task UpdateAsync(Reserva reserva) => _context.SaveChangesAsync();

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
                throw new ConflitoException("Este assento já está ocupado.");
            }
        }
    }
}
