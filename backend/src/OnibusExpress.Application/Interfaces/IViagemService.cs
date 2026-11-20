using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;

namespace OnibusExpress.Application.Interfaces
{
    public interface IViagemService
    {
        Task<IEnumerable<ViagemResumoDto>> BuscarAsync(string? origem, string? destino, DateTime? data);
        Task<ViagemDetalheDto> DetalhesAsync(Guid id);
    }
}
