using System.Collections.Generic;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Application.Mappings
{
    // Mapeamento manual entidade -> DTO (poucos tipos; não justifica AutoMapper)
    public static class DtoMappings
    {
        public static ReservaCriadaDto ToCriadaDto(this Reserva r) => new(r.CodigoReserva, r.Status.ToString());

        public static ReservaDetalheDto ToDetalheDto(this Reserva r) => new(
            r.CodigoReserva, r.Status.ToString(), r.NumeroAssento,
            new ReservaViagemDto(r.Viagem.Rota.Origem, r.Viagem.Rota.Destino, r.Viagem.DataHoraPartida),
            new ReservaPassageiroDto(r.Passageiro.Nome, r.Passageiro.Cpf.Mascarado()));

        public static ViagemResumoDto ToResumoDto(this Viagem v) =>
            new(v.Id, v.Rota.Origem, v.Rota.Destino, v.DataHoraPartida, v.PrecoBase, v.TotalAssentos);

        public static ViagemDetalheDto ToDetalheDto(this Viagem v, IReadOnlyList<int> assentosOcupados) =>
            new(v.Id, v.Rota.Origem, v.Rota.Destino, v.DataHoraPartida, v.PrecoBase, v.TotalAssentos,
                assentosOcupados, v.TotalAssentos - assentosOcupados.Count);

        public static RotaDto ToDto(this Rota r) => new(r.Id, r.Origem, r.Destino, r.DuracaoEstimada);
    }
}
