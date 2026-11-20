using System;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IViagemRepository
    {
        Task<Viagem> GetByIdAsync(Guid id);
    }
}
