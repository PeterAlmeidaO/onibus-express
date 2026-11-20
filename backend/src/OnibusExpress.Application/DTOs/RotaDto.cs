using System;

namespace OnibusExpress.Application.DTOs
{
    public record RotaDto(Guid Id, string Origem, string Destino, TimeSpan DuracaoEstimada);
}
