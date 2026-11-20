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
        [ProducesResponseType(typeof(ReservaCriadaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReservaCriadaDto>> Criar([FromBody] CriarReservaDto dto)
        {
            var reserva = await _reservaService.CriarReservaAsync(dto);
            return CreatedAtAction(nameof(ConsultarPorCodigo), new { codigo = reserva.CodigoReserva }, reserva);
        }

        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(ReservaDetalheDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservaDetalheDto>> ConsultarPorCodigo([FromRoute] string codigo)
        {
            return Ok(await _reservaService.ConsultarPorCodigoAsync(codigo));
        }

        [HttpDelete("{codigo}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarReserva([FromRoute] string codigo)
        {
            await _reservaService.CancelarAsync(codigo);
            return NoContent();
        }
    }
}
