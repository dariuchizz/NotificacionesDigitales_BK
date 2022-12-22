using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/eventos-comunicaciones")]
    [ApiController]
    public class EventosComunicacionesController : ControllerBase
    {
        private readonly ILogger<EventosComunicacionesController> _logger;
        private readonly IEventosComunicacionesServices _eventosComunicacionesServices;
        private readonly IConfiguration _configuration;

        public EventosComunicacionesController(ILogger<EventosComunicacionesController> logger, IEventosComunicacionesServices eventosComunicacionesServices, IConfiguration configuration)
        {
            _logger = logger;
            _eventosComunicacionesServices = eventosComunicacionesServices;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{idComunicacion}")]
        [Authorize("notif_comunicaciones")]
        [SwaggerOperation(Summary = "EventosComunicaciones",
            Description = "Eventos de una comunicacion")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo los eventos de una comunicación",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener los eventos de una comunicación",
            typeof(ServiceResponse<IEnumerable<EventoComunicacionDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<EventoComunicacionDto>>>> GetAsync([FromRoute] long idComunicacion)
        {
            var comunicaciones = await _eventosComunicacionesServices.GetEventosByIdComunicacionAsync(idComunicacion);
            if (comunicaciones.Status != ServiceResponseStatus.Ok)
            {
                return BadRequest(comunicaciones);
            }
            return Ok(comunicaciones);
        }
    }
}