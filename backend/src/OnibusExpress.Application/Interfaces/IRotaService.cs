using System.Collections.Generic;
using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;

namespace OnibusExpress.Application.Interfaces
{
    public interface IRotaService
    {
        Task<IEnumerable<RotaDto>> ListarTodasAsync();
    }
}
