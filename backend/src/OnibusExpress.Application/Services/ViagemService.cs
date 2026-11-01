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

        // ponytail: fuso fixo da operação; tornar configurável se houver rotas em outros fusos
        private static readonly TimeZoneInfo FusoBrasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public async Task<IEnumerable<ViagemResumoDto>> BuscarAsync(string? origem, string? destino, DateTime? data)
        {
            // Viagens que já partiram não são mais ofertadas na busca
            var inicio = DateTime.UtcNow;
            DateTime? fim = null;

            if (data.HasValue)
            {
                // A data informada é um dia do calendário no horário de Brasília; convertido para a janela UTC equivalente
                var inicioDoDia = DateTime.SpecifyKind(data.Value.Date, DateTimeKind.Unspecified);
                var inicioDoDiaUtc = TimeZoneInfo.ConvertTimeToUtc(inicioDoDia, FusoBrasilia);
                fim = TimeZoneInfo.ConvertTimeToUtc(inicioDoDia.AddDays(1), FusoBrasilia);

                if (inicioDoDiaUtc > inicio) inicio = inicioDoDiaUtc;
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
