using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IReservaRepository
    {
        Task<bool> ExisteAssentoConfirmadoAsync(Guid viagemId, int numeroAssento);
        Task<IEnumerable<int>> GetAssentosConfirmadosAsync(Guid viagemId);

        // Carrega viagem (com rota) e passageiro
        Task<Reserva?> GetByCodigoAsync(string codigo);

        // Persiste a reserva junto com o passageiro (novo ou alterado) referenciado por ela
        Task AddAsync(Reserva reserva);
        Task UpdateAsync(Reserva reserva);
    }
}
