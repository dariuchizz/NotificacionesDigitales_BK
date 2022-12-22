using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/envioSMS")]
    [ApiController]
    public class EnviosSMSGenericoController : ControllerBase
    {
        private readonly ILogger<ComunicacionController> _logger;
        private readonly IEnvioSMSGenericosServices _envioSMSGenericoServices;

        public EnviosSMSGenericoController(ILogger<ComunicacionController> logger, IEnvioSMSGenericosServices envioSMSGenericoServices)
        {
            _logger = logger;
            _envioSMSGenericoServices = envioSMSGenericoServices;
        }

        [HttpPost]
        [Route("")]
        [Authorize("notif_envio_sms")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Envio generico de SMS ")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
typeof(ServiceResponse<long>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al intentar enviar el SMS",
typeof(ServiceResponse<long>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<long>>> AddAsync([FromBody] EnvioSMSRequest envioSmsGenerico)
        {
            var response = await _envioSMSGenericoServices.AddEnvioSMSGenericoAsync(envioSmsGenerico);
            return Ok(response);
        }
    }
}
