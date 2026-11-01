using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Infrastructure.Context;

namespace OnibusExpress.Infrastructure.Repositories
{
    public class RotaRepository : IRotaRepository
    {
        private readonly AppDbContext _context;

        public RotaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rota>> GetAllAsync() => await _context.Rotas.AsNoTracking().ToListAsync();
    }
}
