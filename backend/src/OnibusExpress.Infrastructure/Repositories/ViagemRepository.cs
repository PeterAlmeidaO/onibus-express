using System;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Infrastructure.Context;

namespace OnibusExpress.Infrastructure.Repositories
{
    public class ViagemRepository : IViagemRepository
    {
        private readonly AppDbContext _context;

        public ViagemRepository(AppDbContext context)
        {
            _context = context;
        }

        // Rastreada: a viagem é referenciada pela nova reserva no mesmo contexto
        public async Task<Viagem?> GetByIdAsync(Guid id) => await _context.Viagens.FindAsync(id);
    }
}
