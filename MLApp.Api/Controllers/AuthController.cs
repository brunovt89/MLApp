using Microsoft.AspNetCore.Mvc;
using MLApp.Api.DTOs;
using MLApp.Api.Filters;
using MLApp.Api.Services;

namespace MLApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class AuthController : ControllerBase
    {
        private readonly IDataService _dataService;

        public AuthController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request.NumeroDocumento <= 0)
            {
                return BadRequest(new { message = "El número de documento es inválido." });
            }

            var trabajador = await _dataService.LoginAsync(request.NumeroDocumento, request.FechaNacimiento);
            if (trabajador == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas. Verifique el DNI y la Fecha de Nacimiento." });
            }

            return Ok(new 
            { 
                message = "Autenticación exitosa",
                trabajador 
            });
        }
    }
}
