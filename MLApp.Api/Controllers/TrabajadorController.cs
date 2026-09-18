using Microsoft.AspNetCore.Mvc;
using MLApp.Api.Filters;
using MLApp.Api.Services;

namespace MLApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class TrabajadorController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TrabajadorController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("{dni}")]
        public async Task<IActionResult> GetByDni(int dni)
        {
            var trabajador = await _dataService.ObtenerTrabajadorPorDniAsync(dni);
            if (trabajador == null)
            {
                return NotFound(new { message = $"No se encontro ningun trabajador con DNI {dni}." });
            }

            return Ok(trabajador);
        }
    }
}