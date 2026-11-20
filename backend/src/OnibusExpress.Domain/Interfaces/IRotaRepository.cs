using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IRotaRepository
    {
        Task<IEnumerable<Rota>> GetAllAsync();
    }
}
