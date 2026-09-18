using Microsoft.AspNetCore.Mvc;
using MLApp.Api.Filters;
using MLApp.Api.Services;

namespace MLApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class ExamenController : ControllerBase
    {
        private readonly IDataService _dataService;

        public ExamenController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("PorDocumento/{dni}")]
        public async Task<IActionResult> GetExamenes(int dni)
        {
            var examenes = await _dataService.ObtenerExamenesPorDniAsync(dni);
            return Ok(examenes);
        }
    }
}
