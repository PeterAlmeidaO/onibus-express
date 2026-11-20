using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Infrastructure.Context;

namespace OnibusExpress.Infrastructure.Repositories
{
    public class PassageiroRepository : IPassageiroRepository
    {
        private readonly AppDbContext _context;

        public PassageiroRepository(AppDbContext context)
        {
            _context = context;
        }

        // Rastreado: alterações de contato são gravadas junto com a reserva
        public async Task<Passageiro?> GetByCpfAsync(string cpf) =>
            await _context.Passageiros.FirstOrDefaultAsync(p => p.Cpf.Numero == cpf);
    }
}
