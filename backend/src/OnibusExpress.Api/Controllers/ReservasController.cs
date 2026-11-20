using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/reservas")]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarReservaDto dto)
        {
            var reserva = await _reservaService.CriarReservaAsync(dto);
            return CreatedAtAction(nameof(ConsultarPorCodigo), new { codigo = reserva.CodigoReserva }, reserva);
        }

        [HttpGet("{codigo}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConsultarPorCodigo([FromRoute] string codigo)
        {
            return Ok(await _reservaService.ConsultarPorCodigoAsync(codigo));
        }

        [HttpDelete("{codigo}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelarReserva([FromRoute] string codigo)
        {
            await _reservaService.CancelarAsync(codigo);
            return NoContent();
        }
    }
}
