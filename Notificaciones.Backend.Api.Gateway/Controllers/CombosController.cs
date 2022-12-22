using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Response;
using Common.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/combos")]
    [Authorize("notif_combos")]
    [ApiController]
    public class CombosController : ControllerBase
    {
        private readonly ILogger<CombosController> _logger;
        private readonly ICombosServices _combosServices;
        //private readonly IClaseCampaniaServices _claseCampaniaServices;
        private readonly IMemoryCache _memoryCache;
        private const string KeyCanales = "KEYCANALES";
        private const string KeyEstadosCampania = "KEYESTADOSCAMPANIA";
        private const string KeyEstadosSuministro = "KEYESTADOSSUMINISTRO";
        private const string KeyCategoriasSuministro = "KEYCATEGORIASSUMINISTRO";
        private const string KeyLocalidades = "KEYLOCALIDADES";
        private const string KeyEstadosSuministroAgrupacion = "KEYESTADOSSUMINISTROAGRUPACION";
        private const string KeyBusinessUnits = "KEYBUSINESSUNITS";

        public CombosController(ILogger<CombosController> logger, ICombosServices combosServices, IMemoryCache memoryCache)
        {
            _logger = logger;
            _combosServices = combosServices;
            _memoryCache = memoryCache;
            //_claseCampaniaServices = claseCampaniaServices;
        }

        [HttpGet]
        [Route("eventosResultantesEmails")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención eventos resultantes mail")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo eventos resultantes mail",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener eventos resultantes mail",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ComboLongDto>>>> TipoComunicacionesWithEnviosAsync()
        {
            var response = await _combosServices.GetComboEventosResultantesEmailsAsync();
            return response;
        }

        [HttpGet]
        [Route("canales")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención canales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo canales",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener canales",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> CanalesAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyCanales, async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                    return await _combosServices.GetComboCanalesAsync();
                });
            return response;
        }

        [HttpGet]
        [Route("estadosCampania")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención estados campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo estados campania",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener estados campania",
            typeof(ServiceResponse<IEnumerable<ComboLongDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> EstadosCampaniaAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyEstadosCampania, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                return await _combosServices.GetComboEstadosCampaniaAsync();
            });
            return response;
        }
        [HttpGet]
        [Route("estadosSuministro")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención estados del suministro")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo estados del suministro",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener estados del suministro",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> EstadosSuministroAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyEstadosSuministro, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                return await _combosServices.GetComboEstadosSuministroAsync();
            });
            return response;
        }
        [HttpGet]
        [Route("categoriasSuministro")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención categorias del suministro")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo categorias del suministro",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener categorias del suministro",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> CategoriasSuministroAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyCategoriasSuministro, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                return await _combosServices.GetComboCategoriasSuministroAsync();
            });
            return response;
        }

        [HttpGet]
        [Route("localidades")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención de Localidades")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo localidades",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener localidades",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> LocalidadesByBusinessUnitsAsync([FromQuery] string businessUnits)
        {
            if (string.IsNullOrEmpty(businessUnits))
            {
                return ServiceResponseFactory.CreateOkResponse<IEnumerable<ComboStringDto>>(null);
            }
            var finalBusinessUnits = businessUnits.Split(',').ToList();
            var response = await _combosServices.GetComboLocalidadesByBusinessUnitAsync(finalBusinessUnits);
            //var response = await _memoryCache.GetOrCreateAsync(KeyLocalidades, async entry =>
            //{
            //    entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
            //    return await _combosServices.GetComboLocalidadesByBusinessUnitAsync(finalBusinessUnits);
            //});
            return response;
        }

        [HttpGet]
        [Route("estadosSuministro/agrupacion")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención estados del suministro")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo estados del suministro",
            typeof(ServiceResponse<IEnumerable<ComboAgrupacionStringDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener estados del suministro",
            typeof(ServiceResponse<IEnumerable<ComboAgrupacionStringDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboAgrupacionStringDto>>> EstadosSuministroAgrupacionAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyEstadosSuministroAgrupacion, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                return await _combosServices.GetComboEstadosSuministroAgrupacionAsync();
            });
            return response;
        }

        [HttpGet]
        [Route("businessUnits")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención de Unidades de Negocio")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Unidades de Negocio",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Unidades de Negocio",
            typeof(ServiceResponse<IEnumerable<ComboStringDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> BusinessUnitsAsync()
        {
            var response = await _memoryCache.GetOrCreateAsync(KeyBusinessUnits, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                return await _combosServices.GetComboBusinessUnitAsync();
            });
            return response;
        }
        [HttpGet]
        [Route("categorias/cliente")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención de Categorias segun Cliente")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Categorias segun Cliente",
            typeof(ServiceResponse<IEnumerable<ComboGrupoCategoriasDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Categorias segun Cliente",
            typeof(ServiceResponse<IEnumerable<ComboGrupoCategoriasDto>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<IEnumerable<ComboGrupoCategoriasDto>>> CategoriasClienteAsync()
        {
            var response = await _combosServices.GetComboGrupoCategoriasAsync();
            //var response = await _memoryCache.GetOrCreateAsync(KeyBusinessUnits, async entry =>
            //{
            //    entry.SetAbsoluteExpiration(TimeSpan.FromHours(2));
            //    return await _combosServices.GetComboBusinessUnitAsync();
            //});
            return response;
        }

        [HttpGet]
        [Route("clasecampania")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Clase Compania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Clase Compania", typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Clase Compania", typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ClaseCampaniaResponse>>>> ClaseCampaniaAsync()
        {
            var response = await _combosServices.GetComboClasesCampaniasAsync();
            return Ok(response);
        }
    }
}
