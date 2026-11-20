using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IViagemRepository
    {
        Task<Viagem> GetByIdAsync(Guid id);
        Task<IEnumerable<Viagem>> BuscarAsync(string origem, string destino, DateTime? partidaInicio, DateTime? partidaFim);
    }
}
