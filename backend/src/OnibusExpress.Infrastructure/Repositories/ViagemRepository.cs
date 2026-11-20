using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task<Viagem?> GetByIdAsync(Guid id) =>
            await _context.Viagens.Include(v => v.Rota).FirstOrDefaultAsync(v => v.Id == id);

        public async Task<IEnumerable<Viagem>> BuscarAsync(string? origem, string? destino, DateTime? partidaInicio, DateTime? partidaFim)
        {
            var query = _context.Viagens.AsNoTracking().Include(v => v.Rota).AsQueryable();

            if (!string.IsNullOrEmpty(origem))
                query = query.Where(v => v.Rota.Origem.ToLower().Contains(origem.ToLower()));

            if (!string.IsNullOrEmpty(destino))
                query = query.Where(v => v.Rota.Destino.ToLower().Contains(destino.ToLower()));

            if (partidaInicio.HasValue)
                query = query.Where(v => v.DataHoraPartida >= partidaInicio.Value);

            if (partidaFim.HasValue)
                query = query.Where(v => v.DataHoraPartida < partidaFim.Value);

            return await query.ToListAsync();
        }
    }
}
