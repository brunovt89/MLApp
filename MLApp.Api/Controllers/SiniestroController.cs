using Microsoft.AspNetCore.Mvc;
using MLApp.Api.Filters;
using MLApp.Api.Services;

namespace MLApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class SiniestroController : ControllerBase
    {
        private readonly IDataService _dataService;

        public SiniestroController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("PorDocumento/{dni}")]
        public async Task<IActionResult> GetSiniestros(int dni)
        {
            var siniestros = await _dataService.ObtenerSiniestrosPorDniAsync(dni);
            return Ok(siniestros);
        }

        [HttpGet("{siniestro}/Formularios")]
        public async Task<IActionResult> GetFormularios(int siniestro)
        {
            var formularios = await _dataService.ObtenerFormulariosPorSiniestroAsync(siniestro);
            return Ok(formularios);
        }
    }
}