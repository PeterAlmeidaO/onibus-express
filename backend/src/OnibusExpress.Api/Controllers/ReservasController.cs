using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Criar([FromBody] CriarReservaRequest request)
        {
            if (!CpfValidator.Validar(request.Cpf))
                return BadRequest(new { erro = "CPF inválido." });

            try
            {
                var reserva = await _reservaService.CriarReservaAsync(
                    request.ViagemId, request.Nome, request.Cpf, 
                    request.Email, request.DataNascimento, request.NumeroAssento
                );

                return CreatedAtAction(nameof(ConsultarPorCodigo), new { codigo = reserva.CodigoReserva }, new { reserva.CodigoReserva, reserva.Status });
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { erro = ex.Message }); }
            catch (Exception ex)
            {
                // Detalhe vai só para o log; o cliente recebe mensagem genérica
                _logger.LogError(ex, "Erro ao criar reserva para a viagem {ViagemId}", request.ViagemId);
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
                var reserva = await _reservaService.ConsultarPorCodigoAsync(codigo);

                return Ok(new
                {
                    Codigo = reserva.CodigoReserva, Status = reserva.Status, Assento = reserva.NumeroAssento,
                    Viagem = new { Origem = reserva.Viagem.Rota.Origem, Destino = reserva.Viagem.Rota.Destino, Partida = reserva.Viagem.DataHoraPartida },
                    Passageiro = new { Nome = reserva.Passageiro.Nome, Cpf = reserva.Passageiro.Cpf.Mascarado() }
                });
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

    public record CriarReservaRequest(Guid ViagemId, string Nome, string Cpf, string Email, DateTime DataNascimento, int NumeroAssento);
}