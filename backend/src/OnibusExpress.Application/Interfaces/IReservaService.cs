using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;

namespace OnibusExpress.Application.Interfaces
{
    public interface IReservaService
    {
        Task<ReservaCriadaDto> CriarReservaAsync(CriarReservaDto dto);
        Task<ReservaDetalheDto> ConsultarPorCodigoAsync(string codigo);
        Task CancelarAsync(string codigo);
    }
}
