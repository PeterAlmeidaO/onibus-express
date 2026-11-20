using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using OnibusExpress.Application.Mappings;
using OnibusExpress.Domain.Interfaces;

namespace OnibusExpress.Application.Services
{
    public class RotaService : IRotaService
    {
        private readonly IRotaRepository _rotaRepository;

        public RotaService(IRotaRepository rotaRepository)
        {
            _rotaRepository = rotaRepository;
        }

        public async Task<IEnumerable<RotaDto>> ListarTodasAsync()
        {
            var rotas = await _rotaRepository.GetAllAsync();
            return rotas.Select(r => r.ToDto());
        }
    }
}
