using System.Collections.Generic;
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
    [Route("api/unsuscribe")]
    [ApiController]
    public class UnsuscribeController : ControllerBase
    {
        private readonly ILogger<UnsuscribeController> _logger;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly IListaGrisServices _listagrisServices;
        private readonly IMotivoBajaListaGrisServices _motivoBajaListaGrisServices;

        public UnsuscribeController(ILogger<UnsuscribeController> logger, 
            IComunicacionServices comunicacionServices,
            IListaGrisServices listagrisServices,
            IMotivoBajaListaGrisServices motivoBajaListaGrisServices)
        {
            _logger = logger;
            _comunicacionServices = comunicacionServices;
            _listagrisServices = listagrisServices;
            _motivoBajaListaGrisServices = motivoBajaListaGrisServices;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Baja de la comunicación")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo la baja a un comunicación",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al generar la baja de la comunicación",
            typeof(IActionResult))]
        public async Task<ActionResult<ServiceResponse<bool>>> UnsuscribeAsync(
            [FromBody] UnsuscribeDto unsuscribeDto)
        {
            var responseComunicacion = await _comunicacionServices.GetAsync(unsuscribeDto.IdComunicacion, unsuscribeDto.GUID);
            if (responseComunicacion == null)
            {
                //Verifico si existe en la tabla comunicaciones el IdComunicacion y el GUID
                return BadRequest(ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                {
                    new ServiceResponseError
                        {Message = $"Los datos no son correctos"}
                }));
            }
            else {
                var responseListaGris = await _listagrisServices.GetAsync(unsuscribeDto.IdComunicacion);
                if (responseListaGris != null && responseListaGris.Activo == true)
                {
                    //Verifico si la comunicación ya fue ingresada con anterioridad
                    return BadRequest(ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                        new ServiceResponseError
                            {Message = $"La baja de la comunicación ya se realizó con anterioridad"}
                    }));
                }
                else {
                    //Doy de alta la desuscripción
                    var response = await _listagrisServices.AddByStoreAsync(unsuscribeDto);
                }
            }
            return Ok(true);
        }

        [HttpGet]
        [Route("motivos")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención de los motivos para el unsuscribe")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo los motivos para el unsuscribe",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener los motivos",
            typeof(IActionResult))]
        public async Task<ActionResult<ServiceResponse<IEnumerable<MotivoBajaListaGrisResponse>>>> MotivoBajaListaGrisAsync()
        {
            var response = await _motivoBajaListaGrisServices.GetAllAsync();
            return response;
        }
    }
}
