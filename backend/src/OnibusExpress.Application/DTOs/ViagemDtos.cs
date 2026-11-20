using System;
using System.Collections.Generic;

namespace OnibusExpress.Application.DTOs
{
    public record ViagemResumoDto(Guid Id, string Origem, string Destino, DateTime DataHoraPartida, decimal PrecoBase, int TotalAssentos);

    public record ViagemDetalheDto(Guid Id, string Origem, string Destino, DateTime DataHoraPartida, decimal PrecoBase, int TotalAssentos,
                                   IReadOnlyList<int> AssentosOcupados, int VagasRestantes);
}
