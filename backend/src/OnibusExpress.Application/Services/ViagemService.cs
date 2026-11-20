using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;

namespace OnibusExpress.Application.Services
{
    public class ViagemService
    {
        private readonly IViagemRepository _viagemRepository;
        private readonly IReservaRepository _reservaRepository;

        public ViagemService(IViagemRepository viagemRepository, IReservaRepository reservaRepository)
        {
            _viagemRepository = viagemRepository;
            _reservaRepository = reservaRepository;
        }

        public Task<IEnumerable<Viagem>> BuscarAsync(string? origem, string? destino, DateTime? data)
        {
            if (!data.HasValue)
                return _viagemRepository.BuscarAsync(origem, destino, null, null);

            // Força a data recebida a ser tratada como UTC e busca a janela de 24 horas daquele dia
            var inicio = DateTime.SpecifyKind(data.Value.Date, DateTimeKind.Utc);
            return _viagemRepository.BuscarAsync(origem, destino, inicio, inicio.AddDays(1));
        }

        public async Task<(Viagem Viagem, IReadOnlyList<int> AssentosOcupados)> DetalhesAsync(Guid id)
        {
            var viagem = await _viagemRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Viagem não encontrada.");

            var assentosOcupados = (await _reservaRepository.GetAssentosConfirmadosAsync(id)).ToList();

            return (viagem, assentosOcupados);
        }
    }
}
