using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using System;
using System.Collections.Generic;
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
        [ProducesResponseType(typeof(IEnumerable<ViagemResumoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ViagemResumoDto>>> Buscar([FromQuery] string? origem, [FromQuery] string? destino, [FromQuery] DateTime? data)
        {
            return Ok(await _viagemService.BuscarAsync(origem, destino, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ViagemDetalheDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ViagemDetalheDto>> Detalhes(Guid id)
        {
            return Ok(await _viagemService.DetalhesAsync(id));
        }
    }
}
