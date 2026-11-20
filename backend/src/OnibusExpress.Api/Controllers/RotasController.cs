using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Interfaces;
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
        public async Task<IActionResult> ListarTodas()
        {
            return Ok(await _rotaService.ListarTodasAsync());
        }
    }
}
