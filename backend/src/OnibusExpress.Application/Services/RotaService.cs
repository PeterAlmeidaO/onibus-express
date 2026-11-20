using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Interfaces;

namespace OnibusExpress.Application.Services
{
    public class RotaService
    {
        private readonly IRotaRepository _rotaRepository;

        public RotaService(IRotaRepository rotaRepository)
        {
            _rotaRepository = rotaRepository;
        }

        public Task<IEnumerable<Rota>> ListarTodasAsync() => _rotaRepository.GetAllAsync();
    }
}
