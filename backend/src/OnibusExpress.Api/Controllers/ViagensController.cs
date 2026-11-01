using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/viagens")]
    public class ViagensController : ControllerBase
    {
        private readonly IViagemService _viagemService;

        public ViagensController(IViagemService viagemService)
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
            return Ok(await _viagemService.DetalhesAsync(id));
        }
    }
}
