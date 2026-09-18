using Microsoft.AspNetCore.Mvc;
using MLApp.Api.Filters;
using MLApp.Api.Services;

namespace MLApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class TurnoController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TurnoController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("PorDocumento/{dni}")]
        public async Task<IActionResult> GetTurnos(int dni, [FromQuery] bool soloFuturos = false)
        {
            var turnos = await _dataService.ObtenerTurnosPorDniAsync(dni, soloFuturos);
            return Ok(turnos);
        }
    }
}
