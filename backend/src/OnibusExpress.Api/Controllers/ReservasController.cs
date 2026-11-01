using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Api.Validations;
using System;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/reservas")]
    public class ReservasController : ControllerBase
    {
        private readonly ReservaApplicationService _reservaService;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(ReservaApplicationService reservaService, ILogger<ReservasController> logger)
        {
            _reservaService = reservaService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarReservaDto dto)
        {
            if (!CpfValidator.Validar(dto.Cpf))
                return BadRequest(new { erro = "CPF inválido." });

            try
            {
                var reserva = await _reservaService.CriarReservaAsync(dto);

                return CreatedAtAction(nameof(ConsultarPorCodigo), new { codigo = reserva.CodigoReserva }, reserva);
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { erro = ex.Message }); }
            catch (Exception ex)
            {
                // Detalhe vai só para o log; o cliente recebe mensagem genérica
                _logger.LogError(ex, "Erro ao criar reserva para a viagem {ViagemId}", dto.ViagemId);
                return StatusCode(500, new { erro = "Erro interno ao processar a reserva." });
            }
        }

        [HttpGet("{codigo}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConsultarPorCodigo([FromRoute] string codigo)
        {
            try
            {
                return Ok(await _reservaService.ConsultarPorCodigoAsync(codigo));
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }

        [HttpDelete("{codigo}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelarReserva([FromRoute] string codigo)
        {
            try
            {
                await _reservaService.CancelarAsync(codigo);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
        }
    }
}
