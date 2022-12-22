using System.Collections.Generic;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/tipoComunicacion")]
    [ApiController]
    public class TipoComunicacionController : ControllerBase
    {
        private readonly ILogger<TipoComunicacionController> _logger;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;

        public TipoComunicacionController(ILogger<TipoComunicacionController> logger, ITipoComunicacionServices tipoComunicacionServices)
        {
            _logger = logger;
            _tipoComunicacionServices = tipoComunicacionServices;
        }

        [HttpGet]
        [Route("")]
        //[Authorize("repo_tipo_comunicaciones")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención Tipo Comunicaciones")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Tipo Comunicaciones con envios",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Tipo Comunicaciones",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<TipoComunicacionesResponse>>>> TipoComunicacionesWithEnviosAsync()
        {
            var response = await _tipoComunicacionServices.GetAllAsync();
            return response;
        }

    }
}
