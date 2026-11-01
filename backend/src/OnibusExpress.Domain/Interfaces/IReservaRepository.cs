using System;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IReservaRepository
    {
        Task<bool> ExisteAssentoConfirmadoAsync(Guid viagemId, int numeroAssento);

        // Persiste a reserva junto com o passageiro (novo ou alterado) referenciado por ela
        Task AddAsync(Reserva reserva);
    }
}
