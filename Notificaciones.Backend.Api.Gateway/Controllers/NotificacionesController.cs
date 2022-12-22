using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/notificaciones")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly ILogger<NotificacionesController> _logger;
        private readonly INotificacionesServices _notificacionesServices;
        private readonly IConfiguration _configuration;

        public NotificacionesController(ILogger<NotificacionesController> logger, INotificacionesServices notificacionesServices, IConfiguration configuration)
        {
            _logger = logger;
            _notificacionesServices = notificacionesServices;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{comunicacion}")]
        [SwaggerOperation(Summary = "Notificaciones",
            Description = "Eventos segun el numero de comprobante")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo eventos segun el numero de comprobante",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener eventos segun el numero de comprobante",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<EventoComunicacionDto>>>> GetAsync([FromRoute] ComunicacionType comunicacion,
            [FromQuery] string numeroComprobante)
        {
            if (string.IsNullOrEmpty(numeroComprobante))
            {
                return BadRequest(ServiceResponseFactory.CreateErrorResponse<IEnumerable<EventoComunicacionDto>>(new[]
                {

                    new ServiceResponseError
                        {Message = $"No se ha especificado el Comprobante"}
                }));
            }
            var maxTake = _configuration.GetSection("Notificaciones").GetSection("Por-Comprobante")
                .GetValue<int>("Max-Take");

            var comunicaciones = await _notificacionesServices.NotificacionesAsync(comunicacion, numeroComprobante, maxTake);
            return Ok(comunicaciones);
        }

        [HttpGet]
        [Route("{comunicacion}/{idComprobante}")]
        [SwaggerOperation(Summary = "Notificaciones",
            Description = "Eventos segun el id interno de comprobante")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo eventos segun el id interno de comprobante",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener eventos segun el id interno de comprobante",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<EventoComunicacionDto>>>> GetAsync([FromRoute] ComunicacionType comunicacion,
            [FromRoute] long idComprobante)
        {
            var maxTake = _configuration.GetSection("Notificaciones").GetSection("Por-Comprobante")
                .GetValue<int>("Max-Take");
            var comunicaciones = await _notificacionesServices.NotificacionesAsync(comunicacion, idComprobante, maxTake);
            return Ok(comunicaciones);
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation(Summary = "Notificaciones",
            Description = "Comunicaciones y eventos según cuenta suministro")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo comunicaciones y eventos según cuenta suministro",
            typeof(ServiceResponse<IEnumerable<ReporteEventosPorCuentaDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener comunicaciones y eventos según cuenta suministro",
            typeof(ServiceResponse<IEnumerable<ReporteEventosPorCuentaDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ReporteEventosPorCuentaDto>>>> GetAsync([FromQuery] string cuentaUnificada)
        {
            var mesesParatAtras = _configuration.GetSection("Notificaciones").GetSection("Por-Cuenta")
                .GetValue<int>("Month-To-Back");
            var cuentaUnificadaLong = cuentaUnificada.Replace("-", "").Replace("/", "");
            var comunicaciones = await _notificacionesServices.NotificacionesAsync(cuentaUnificadaLong, mesesParatAtras);
            if (comunicaciones.Status != ServiceResponseStatus.Ok)
            {
                return BadRequest(comunicaciones);
            }
            return Ok(comunicaciones);
        }

        [HttpGet]
        [Route("Campanias")]
        [Authorize("notif_comunicaciones")]
        [SwaggerOperation(Summary = "Notificaciones",
            Description = "Comunicaciones de tipo campaña según una cuenta unificada")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo comunicaciones de tipo camapaña según una cuenta unificada",
            typeof(ServiceResponse<NotifiacionCampaniaResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener las comunicaciones de tipo campaña",
            typeof(ServiceResponse<NotifiacionCampaniaResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<NotifiacionCampaniaResponse>>> GetNotificacionesCampaniaAsync([FromQuery] NotificacionRequest request)
        {
            request.cuentaUnificada = request.cuentaUnificada.Replace("-", "").Replace("/", "");
            var comunicaciones = await _notificacionesServices.NotificacionesCampaniaAsync(request);
            if (comunicaciones.Status != ServiceResponseStatus.Ok)
            {
                return BadRequest(comunicaciones);
            }
            return Ok(comunicaciones);
        }

        [HttpGet]
        [Route("ProcesosNegocios")]
        [Authorize("notif_comunicaciones")]
        [SwaggerOperation(Summary = "Notificaciones",
            Description = "Comunicaciones de tipo proceso negocio según una cuenta unificada")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo comunicaciones de tipo proceso negocio según una cuenta unificada",
            typeof(ServiceResponse<NotifiacionProcesoNegocioResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener las comunicaciones de tipo proceso negocio",
            typeof(ServiceResponse<NotifiacionProcesoNegocioResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<NotifiacionProcesoNegocioResponse>>> GetNotificacionesProcesoNegocioAsync([FromQuery] NotificacionRequest request)
        {
            request.cuentaUnificada = request.cuentaUnificada.Replace("-", "").Replace("/", "");
            var comunicaciones = await _notificacionesServices.NotificacionesProcesoNegocioAsync(request);
            if (comunicaciones.Status != ServiceResponseStatus.Ok)
            {
                return BadRequest(comunicaciones);
            }
            return Ok(comunicaciones);
        }
    }
}