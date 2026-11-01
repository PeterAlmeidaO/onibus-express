using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/rotas")]
    public class RotasController : ControllerBase
    {
        private readonly IRotaService _rotaService;

        public RotasController(IRotaService rotaService)
        {
            _rotaService = rotaService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RotaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RotaDto>>> ListarTodas()
        {
            return Ok(await _rotaService.ListarTodasAsync());
        }
    }
}
