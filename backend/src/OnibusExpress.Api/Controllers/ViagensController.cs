using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Exceptions;
using System;
using System.Linq;
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
            var viagens = await _viagemService.BuscarAsync(origem, destino, data);

            return Ok(viagens.Select(v => new {
                v.Id,
                Origem = v.Rota.Origem,
                Destino = v.Rota.Destino,
                v.DataHoraPartida,
                v.PrecoBase,
                v.TotalAssentos
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            try
            {
                var (viagem, assentosOcupados) = await _viagemService.DetalhesAsync(id);

                return Ok(new {
                    viagem.Id,
                    Origem = viagem.Rota.Origem,
                    Destino = viagem.Rota.Destino,
                    viagem.DataHoraPartida,
                    viagem.PrecoBase,
                    viagem.TotalAssentos,
                    AssentosOcupados = assentosOcupados,
                    VagasRestantes = viagem.TotalAssentos - assentosOcupados.Count
                });
            }
            catch (NotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }
    }
}
