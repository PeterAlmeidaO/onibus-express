using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using OnibusExpress.Application.Mappings;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;

namespace OnibusExpress.Application.Services
{
    public class ViagemService : IViagemService
    {
        private readonly IViagemRepository _viagemRepository;
        private readonly IReservaRepository _reservaRepository;

        public ViagemService(IViagemRepository viagemRepository, IReservaRepository reservaRepository)
        {
            _viagemRepository = viagemRepository;
            _reservaRepository = reservaRepository;
        }

        public async Task<IEnumerable<ViagemResumoDto>> BuscarAsync(string? origem, string? destino, DateTime? data)
        {
            DateTime? inicio = null, fim = null;

            if (data.HasValue)
            {
                // Força a data recebida a ser tratada como UTC e busca a janela de 24 horas daquele dia
                inicio = DateTime.SpecifyKind(data.Value.Date, DateTimeKind.Utc);
                fim = inicio.Value.AddDays(1);
            }

            var viagens = await _viagemRepository.BuscarAsync(origem, destino, inicio, fim);
            return viagens.Select(v => v.ToResumoDto());
        }

        public async Task<ViagemDetalheDto> DetalhesAsync(Guid id)
        {
            var viagem = await _viagemRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Viagem não encontrada.");

            var assentosOcupados = (await _reservaRepository.GetAssentosConfirmadosAsync(id)).ToList();

            return viagem.ToDetalheDto(assentosOcupados);
        }
    }
}
