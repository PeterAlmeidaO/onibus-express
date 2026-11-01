using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Services;
using System.Threading.Tasks;

namespace OnibusExpress.Api.Controllers
{
    [ApiController]
    [Route("api/rotas")]
    public class RotasController : ControllerBase
    {
        private readonly RotaService _rotaService;

        public RotasController(RotaService rotaService)
        {
            _rotaService = rotaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var rotas = await _rotaService.ListarTodasAsync();
            return Ok(rotas);
        }
    }
}
