using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/viagens")]
    public class ViagensController : ControllerBase
    {
        private readonly ViagemService _viagemService;

        public ViagensController(ViagemService viagemService)
        {
            _viagemService = viagemService;
        }

        [HttpGet]
        public async Task<IActionResult> Buscar([FromQuery] string? origem, [FromQuery] string? destino, [FromQuery] DateTime? data)
        {
            return Ok(await _viagemService.BuscarAsync(origem, destino, data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            try
            {
                return Ok(await _viagemService.DetalhesAsync(id));
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }
    }
}
