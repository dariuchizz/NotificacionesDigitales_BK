using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/motivosbaja")]
    [ApiController]
    public class MotivosBajaController : ControllerBase
    {
        private readonly ILogger<MotivosBajaController> _logger;
        private readonly IMotivoBajaServices _motivoBajaServices;

        public MotivosBajaController(ILogger<MotivosBajaController> logger, IMotivoBajaServices motivoBajaServices)
        {
            _logger = logger;
            _motivoBajaServices = motivoBajaServices;
        }

        [HttpGet]
        [Route("{tipoMotivo}")]
        //[Authorize("repo_cobrabilidad")]
        [SwaggerOperation(Summary = "Motivos de Baja", Description = "Obtención Motivos de Baja segun tipo motivo")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Motivos de Baja segun tipo motivo",
            typeof(ServiceResponse<ComboLongDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Motivos de Baja segun tipo motivo",
            typeof(ServiceResponse<ComboLongDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<ComboLongDto>>> GetByTipoMotivoAsync([FromRoute] string tipoMotivo)
        {
            var response = await _motivoBajaServices.GetComboMotivoBajaByTipoMotivoAsync(tipoMotivo);
            return Ok(response);
        }

    }
}
