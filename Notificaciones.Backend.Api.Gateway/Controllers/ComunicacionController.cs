using System.IO;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/comunicacion")]
    [ApiController]
    public class ComunicacionController : ControllerBase
    {
        private readonly ILogger<ComunicacionController> _logger;
        private readonly IComunicacionServices _comunicacionServices;

        public ComunicacionController(ILogger<ComunicacionController> logger,IComunicacionServices comunicacionServices)
        {
            _logger = logger;
            _comunicacionServices = comunicacionServices;
        }

        [HttpGet]
        [Route("")]
        //[Authorize("repo_cobrabilidad")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
typeof(ServiceResponse<CobrabilidadResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
typeof(ServiceResponse<CobrabilidadResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<ReporteEventosPorCuentaResponse>>> ReportAsync([FromQuery] ReporteEventosPorCuentaRequest request)
        {
            var response = await _comunicacionServices.ReportAsync(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("excel")]
        //[Authorize("repo_cobrabilidad")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención Reporte Notificaciones Digitales Excel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<IActionResult> ExportExcelAsync([FromQuery] ReporteEventosPorCuentaRequest request)
        {
            var response = await _comunicacionServices.ExportExcelAsync(request);
            var memory = new MemoryStream(response.Result.FileStream) { Position = 0 };
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Result.Title);
        }
    }
}
