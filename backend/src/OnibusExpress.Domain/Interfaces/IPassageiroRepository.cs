using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Interfaces
{
    public interface IPassageiroRepository
    {
        Task<Passageiro?> GetByCpfAsync(string cpf);
    }
}
