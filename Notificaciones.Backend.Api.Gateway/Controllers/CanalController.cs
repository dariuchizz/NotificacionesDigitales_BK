using System.Collections.Generic;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/canal")]
    [ApiController]
    public class CanalController : ControllerBase
    {
        private readonly ILogger<CanalController> _logger;
        private readonly ICanalServices _canalServices;

        public CanalController(ILogger<CanalController> logger, ICanalServices canalServices)
        {
            _logger = logger;
            _canalServices = canalServices;
        }

        [HttpGet]
        [Route("canales")]
        [Authorize("repo_canal")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención Canales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Canales",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Canales",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<CanalResponse>>>> CanalesAsync()
        {
            var response = await _canalServices.GetAllAsync();
            return Ok(response);
        }
    }
}
