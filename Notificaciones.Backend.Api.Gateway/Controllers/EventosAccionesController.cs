using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/eventos-acciones")]
    [ApiController]
    public class EventosAccionesController : ControllerBase
    {
        private readonly ILogger<EventosAccionesController> _logger;
        private readonly IEventosAccionesServices _eventosAccionesServices;
        public EventosAccionesController(ILogger<EventosAccionesController> logger,IEventosAccionesServices eventosAccionesServices)
        {
            _logger = logger;
            _eventosAccionesServices = eventosAccionesServices;
        }
        [HttpGet]
        [Authorize("eventos_acciones_view")]
        [Route("")]
        [SwaggerOperation(Summary = "Eventos y Acciones",
            Description = "Obtención de los Eventos y Acciones")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo los Eventos y Acciones",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener los Eventos y Acciones",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operacion no permitida")]
        public async Task<ActionResult<ServiceResponse<GridEventosAccionesResponse>>> GridAsync([FromQuery] GridEventosAccionesRequest request)
        {
            var users = await _eventosAccionesServices.GridAsync(request);
            return Ok(users);
        }

        [HttpGet]
        [Route("excel")]
        [Authorize("eventos_acciones_view")]
        [SwaggerOperation(Summary = "Eventos y Acciones",
            Description = "Obtención Eventos y Acciones Excel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Eventos y Acciones Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Eventos y Acciones Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<IActionResult> ExportExcelAsync([FromQuery] GridEventosAccionesRequest request)
        {
            var response = await _eventosAccionesServices.ExportExcelAsync(request);
            var memory = new MemoryStream(response.Result.FileStream) { Position = 0 };
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Result.Title);
        }

        [HttpGet]
        [Authorize("eventos_acciones_abm")]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Eventos y Acciones",
            Description = "Obtención del Evento y Accione")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo el Evento y Accione",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener el Evento y Accione",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operacion no permitida")]
        public async Task<ActionResult<ServiceResponse<GridEventosAccionesResponse>>> GetAsync([FromRoute] long id)
        {
            var users = await _eventosAccionesServices.GetAsync(id);
            return Ok(users);
        }

        [HttpPut]
        [Authorize("eventos_acciones_abm")]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Eventos y Acciones",
            Description = "Obtención del Evento y Accione")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo el Evento y Accione",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener el Evento y Accione",
            typeof(ServiceResponse<GridEventosAccionesResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operacion no permitida")]
        public async Task<ActionResult<ServiceResponse<GridEventosAccionesResponse>>> PutAsync([FromRoute] long id, EventoAccionesFormularioDto item)
        {
            item.AutorModificacion = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            var users = await _eventosAccionesServices.PutAsync(id, item);
            return Ok(users);
        }
    }
}
