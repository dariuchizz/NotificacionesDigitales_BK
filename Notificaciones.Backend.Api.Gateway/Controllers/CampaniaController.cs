using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Enum;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Processor;
using Processor.Dto;
using Processor.HangfireProcess;
using Swashbuckle.AspNetCore.Annotations;

namespace Notificaciones.Backend.Api.Gateway.Controllers
{
    [Route("api/campanias")]
    [ApiController]
    public class CampaniaController : ControllerBase
    {
        private readonly ILogger<CampaniaController> _logger;
        private readonly ICampaniaServices _campaniaServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProcessManager> _logProcess;
        private readonly IProcessManager _processManager;
        private readonly ICsvCampaniaServices _csvCampaniaServices;
        private readonly IMailgun _mailgun;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;

        public CampaniaController(ILogger<CampaniaController> logger, ICampaniaServices campaniaServices, IConfiguration configuration, ILogger<ProcessManager> logProcess, IProcessManager processManager, ICsvCampaniaServices csvCampaniaServices, IMailgun mailgun, IComunicacionServices comunicacionServices, ITipoComunicacionServices tipoComunicacionServices)
        {
            _logger = logger;
            _campaniaServices = campaniaServices;
            _configuration = configuration;
            _logProcess = logProcess;
            _processManager = processManager;
            _csvCampaniaServices = csvCampaniaServices;
            _mailgun = mailgun;
            _comunicacionServices = comunicacionServices;
            _tipoComunicacionServices = tipoComunicacionServices;
        }

        [HttpGet]
        [Route("")]
        [Authorize("notif_gestor_campanias")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Campanias")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Campanias",
            typeof(ServiceResponse<CampaniaResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Campanias",
            typeof(ServiceResponse<CampaniaResponse>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<CampaniaResponse>>> GridAsync([FromQuery] GridCampaniaRequest request)
        {
            var response = await _campaniaServices.GridAsync(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("excel")]
        [Authorize("notif_gestor_campanias")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención Campanias Excel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Campanias Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Campanias Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<IActionResult> ExportExcelAsync([FromQuery] GridCampaniaRequest request)
        {
            var response = await _campaniaServices.ExportExcelAsync(request);
            var memory = new MemoryStream(response.Result.FileStream) { Position = 0 };
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Result.Title);
        }

        [HttpGet]
        [Route("{idCampania}")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<CampaniaDto>> GetAsync([FromRoute] long idCampania)
        {
            var response = await _campaniaServices.GetWithRelationshipsAsync(idCampania);
            if (string.IsNullOrEmpty(response.TemplateName)) return ServiceResponseFactory.CreateOkResponse(response);
            var mailgunRequest = new MailgunRequest
            {
                Tag = response.Tag,
                Description = response.Descripcion,
                Template = response.TemplateName,
                TemplateHtml = response.TemplateHtml
            };
            var templateMailgun = await _mailgun.GetTemplateAsync(TipoEnvioType.Campania, mailgunRequest, _configuration, _logProcess);
            if (templateMailgun.Template != null)
            {
                response.TemplateHtml = templateMailgun.Template.Version.Template;
            }
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        [HttpPost]
        [Route("")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<CampaniaDto>>> AddAsync([FromBody] CampaniaDto campania)
        {
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            var response = await _campaniaServices.AddCampaignAsync(campania, userLogged);
            return Ok(response);
        }

        [HttpPut]
        [Route("{idCampania}")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<CampaniaDto>>> PutAsync([FromRoute] long idCampania, [FromBody] CampaniaDto campania)
        {
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            var response = await _campaniaServices.UpdateCampaignAsync(campania, userLogged);
            if (campania.ActualStep == 3)
            {
                _logger.LogDebug("Entro por paso 3");
                if (!string.IsNullOrEmpty(campania.TemplateHtml))
                {
                    _logger.LogDebug("Entro por que no tengo template");
                    var mailgunRequest = new MailgunRequest
                    {
                        Tag = campania.Tag,
                        Description = campania.Descripcion,
                        Template = campania.TemplateName?.ToLowerInvariant().Replace(" ", "_"),
                        TemplateHtml = campania.TemplateHtml
                    };
                    var templateMailgun = await _mailgun.GetTemplateAsync(TipoEnvioType.Campania, mailgunRequest, _configuration, _logProcess);
                    _logger.LogDebug($"Mensaje template: {templateMailgun.Message}");

                    if (!string.IsNullOrEmpty(templateMailgun.Message))
                    {
                        if (templateMailgun.Message.Equals("Invalid parameter: name is invalid"))
                        {
                            return ServiceResponseFactory.CreateErrorResponse<CampaniaDto>(new[]
                            {
                                new ServiceResponseError
                                    {Message = $"El nombre del template contiene caracteres inválidos."}
                            });

                        }
                    }
                    if (templateMailgun.Template != null)
                    {
                        _logger.LogDebug($"Elimino el template");
                        var responseDelete = await _mailgun.DeleteTemplateAsync(TipoEnvioType.Campania, mailgunRequest,
                            _configuration, _logProcess);
                    }
                    _logger.LogDebug($"Creo el template");
                    var responseAdded = await _mailgun.AddTemplateAsync(TipoEnvioType.Campania, mailgunRequest,
                        _configuration, _logProcess);
                }
                _logger.LogDebug($"Realice todo OK");
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize("notif_gestor_campanias_formulario")]
        [Route("{idCampania}/execute")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Ejecucion de la campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ejecucion de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error en Ejecucion de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> ExecuteJobAsync([FromRoute] long idCampania)
        {
            if (_processManager.TryGetProcess("generar-campania", out ProcessDto processGenerate))
            {
                try
                {
                    var executor = processGenerate.ProcessFactory();
                    BackgroundJob.Enqueue(() =>
                        executor.ExecuteAsync(idCampania.ToString(), CancellationToken.None)
                    );
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error al realizar la ejecución en la generación de la campaña: {e}");
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {

                        new ServiceResponseError
                            {Message = $"Error al realizar la ejecución en la generación de la campaña"}
                    });
                }
            }
            if (_processManager.TryGetProcess("enviar-campania", out ProcessDto processSend))
            {
                try
                {
                    var date = DateTime.Now.AddMinutes(30);
                    var executor = processSend.ProcessFactory();
                    BackgroundJob.Schedule(() =>
                        executor.ExecuteAsync(idCampania.ToString(), CancellationToken.None), date
                    );
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error al realizar la planificación en el envío de la campaña: {e}");
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {

                        new ServiceResponseError
                            {Message = $"Error al realizar la planificación en el envío de la campaña"}
                    });
                }

                return ServiceResponseFactory.CreateOkResponse(true);
            }
            var campania = await _campaniaServices.GetWithRelationshipsAsync(idCampania);
            campania.IdEstadoCampania = 2;
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            await _campaniaServices.UpdateAsync(campania, userLogged);
            return ServiceResponseFactory.CreateOkResponse(true);
        }

        [HttpPost]
        [Authorize("notif_gestor_campanias_formulario")]
        [Route("{idCampania}/schedule")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Schedule de la campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Schedule de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error en Schedule de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> ScheduleJobAsync([FromRoute] long idCampania, [FromBody] DateTime date)
        {
            var campania = await _campaniaServices.GetAsync(idCampania);
            var dateSelected = date.ToLocalTime();
            if (dateSelected.TimeOfDay < new TimeSpan(16, 0, 0))
            {
                return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                {
                    new ServiceResponseError
                        {Message = $"La hora de planificación debe ser posterior a las 4:00 pm (16:00 hs)"}
                });
            }
            if (date.ToLocalTime() < DateTime.Now.AddHours(2))
            {
                return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                {
                    new ServiceResponseError
                        {Message = $"La fecha de planificación debe ser superior a la fecha actual más 2 horas."}
                });
            }
            var generarJobId = $"generar_{campania.TemplateName.Trim().ToLower()}_{campania.IdCampania}";
            var obtenerJobId = $"generar_{campania.TemplateName.Trim().ToLower()}_{campania.IdCampania}";

            if (_processManager.TryGetProcess("generar-campania", out ProcessDto processGenerar))
            {
                try
                {
                    var executor = processGenerar.ProcessFactory();
                    campania.IdScheduleGenerar = BackgroundJob.Schedule(() =>
                        executor.ExecuteAsync(idCampania.ToString(), CancellationToken.None), date
                    );
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error al realizar la planificación en la generación de la campaña: {e}");
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                        new ServiceResponseError
                            {Message = $"Error al realizar la planificación en la generación de la campaña. Por favor, contáctese con Sistemas."}
                    });
                }
            }
            if (_processManager.TryGetProcess("enviar-campania", out ProcessDto processEnviar))
            {
                try
                {
                    var executor = processEnviar.ProcessFactory();
                    campania.IdScheduleObtener = BackgroundJob.Schedule(() =>
                        executor.ExecuteAsync(idCampania.ToString(), CancellationToken.None), date.AddMinutes(30)
                    );
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error al realizar la planificación en el envío de la campaña: {e}");
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                        new ServiceResponseError
                            {Message = $"Error al realizar la planificacion en el envío de la campaña. " +
                                $"La generación de la campaña ya quedó programada y planificada. Por favor, contáctese con Sistemas"}
                    });
                }
            }
            try
            {
                var response = await _campaniaServices.GetDatosCampaniaAsync(campania);
                var result = response.Result;
                if (result.Any())
                {
                    campania.CantidadEmails = result.FirstOrDefault(f => f.Descripcion == "Emails")?.cantidad ?? 0;
                    campania.CantidadSuministros =
                        result.FirstOrDefault(f => f.Descripcion == "Suministros")?.cantidad ?? 0;
                }
                campania.IdEstadoCampania = 3;
                campania.FechaPlanificado = date.ToLocalTime();
                var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
                await _campaniaServices.UpdateAsync(campania, userLogged);
                return ServiceResponseFactory.CreateOkResponse(true);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al intentar actualizar el estado de la campaña: {e}");
                return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                {
                    new ServiceResponseError
                        {Message = $"Error al intentar actualizar el estado de la campaña. " +
                                $"La misma ya quedó programada y planificada. Por favor, contáctese con Sistemas"}
                });
            }
        }

        [HttpGet]
        [Route("{idCampania}/consultas")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ActionResult<ServiceResponse<CampaniaDto>>> GetDatosCampaniaAsync([FromRoute] long idCampania)
        {
            var campania = await _campaniaServices.GetWithRelationshipsAsync(idCampania);
            var response = await _campaniaServices.GetDatosCampaniaAsync(campania);
            var result = response.Result;
            if (result.Any())
            {
                string descripcion = result.FirstOrDefault().Descripcion;
                campania.CantidadEmails = result.FirstOrDefault(f => f.Descripcion == descripcion).cantidad ?? 0;
                campania.CantidadSuministros =
                    result.FirstOrDefault(f => f.Descripcion == "Suministros").cantidad ?? 0;
                var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
                await _campaniaServices.UpdateAsync(campania, userLogged);
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize("notif_gestor_campanias_formulario")]
        [Route("{idCampania}/test")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Ejecucion de la campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ejecucion de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error en Ejecucion de la campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> TestMailAsync([FromRoute] long idCampania, [FromBody] TestMailgunDto request)
        {
            if (_processManager.TryGetProcess("test-mailgun", out ProcessDto process))
            {
                try
                {
                    if (request.IsByCsv)
                    {
                        var firstRowCsv = await _csvCampaniaServices.GetFirstRowAsync(idCampania);
                        request.ApellidoValor = string.IsNullOrEmpty(firstRowCsv.Apellido) ? request.ApellidoValor : string.Empty;
                        request.NombreValor = string.IsNullOrEmpty(firstRowCsv.Nombre) ? request.NombreValor : string.Empty;
                        request.DomicilioValor = string.IsNullOrEmpty(firstRowCsv.Domicilio) ? request.DomicilioValor : string.Empty;
                        request.NombreCompletoValor = string.IsNullOrEmpty(firstRowCsv.NombreApellido) ? request.NombreCompletoValor : string.Empty;
                    }
                    var parameters = JsonConvert.SerializeObject(request);
                    if (request.HasVariables)
                    {
                        if (!string.IsNullOrEmpty(request.ApellidoJson))
                        {
                            var valor = GetMemberName(() => request.ApellidoValor);
                            parameters = parameters.Replace(valor, request.ApellidoJson);
                        }
                        if (!string.IsNullOrEmpty(request.NombreJson))
                        {
                            var valor = GetMemberName(() => request.NombreValor);
                            parameters = parameters.Replace(valor, request.NombreJson);
                        }
                        if (!string.IsNullOrEmpty(request.NombreCompletoJson))
                        {
                            var valor = GetMemberName(() => request.NombreCompletoValor);
                            parameters = parameters.Replace(valor, request.NombreCompletoJson);
                        }
                        if (!string.IsNullOrEmpty(request.DomicilioJson))
                        {
                            var valor = GetMemberName(() => request.DomicilioValor);
                            parameters = parameters.Replace(valor, request.DomicilioJson);
                        }
                    }
                    var executor = process.ProcessFactory();
                    BackgroundJob.Enqueue(() =>
                        executor.ExecuteAsync(parameters, CancellationToken.None)
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return ServiceResponseFactory.CreateOkResponse(true);
            }
            return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
            {

                new ServiceResponseError
                    {Message = $"Error al realizar la prueba de envío de Mail"}
            });
        }

        [HttpPost]
        //[Authorize("notif_gestor_campanias_formulario")]
        [Route("{idCampania}/testSMS")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Ejecucion de la campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ejecucion de la campania",
           typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error en Ejecucion de la campania",
           typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> TestSMSAsync([FromRoute] long idCampania, [FromBody] TestSMSDto request)
        {
            if (_processManager.TryGetProcess("test-startPlus", out ProcessDto process))
            {
                try
                {
                    if (request.IsByCsv)
                    {
                        var firstRowCsv = await _csvCampaniaServices.GetFirstRowAsync(idCampania);
                        request.NombreValor = firstRowCsv.Nombre;
                    }
                    var parameters = JsonConvert.SerializeObject(request);
                    if (request.HasVariables)
                    {
                        var valor = GetMemberName(() => request.NombreValor);
                    }
                    var executor = process.ProcessFactory();
                    BackgroundJob.Enqueue(() =>
                        executor.ExecuteAsync(parameters, CancellationToken.None)
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return ServiceResponseFactory.CreateOkResponse(true);
            }
            return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
            {

                new ServiceResponseError
                    {Message = $"Error al realizar la prueba de envío de SMS"}
            });
        }

        [HttpDelete]
        [Route("{idCampania}/logic")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Borre logico de la Campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Borre logico de la Campania",
            typeof(ServiceResponse<bool>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al realizar el Borre logico de la Campanias",
            typeof(ServiceResponse<bool>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> DeleteLogicAsync([FromRoute] long idCampania)
        {
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            var response = await _campaniaServices.UpdateActiveAsync(idCampania, false, userLogged);
            return response;
        }

        [HttpPost]
        [Route("{idCampania}/duplicar")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Duplicar campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Duplicar campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al duplicar campania",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> DuplicateAsync([FromRoute] long idCampania)
        {
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            var response = await _campaniaServices.DuplicateAsync(idCampania, userLogged);
            return response;
        }

        [HttpPost]
        [Authorize("notif_gestor_campanias_formulario")]
        [Route("{idCampania}/schedule/cancel")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Cancelar Schedule de la campania")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cancelar Schedule de la campania",
    typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error en Cancelar Schedule de la campania",
    typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<bool>> CancelScheduleJobAsync([FromRoute] long idCampania)
        {
            //var campania = await _campaniaServices.GetWithRelationshipsAsync(idCampania);
            var campania = await _campaniaServices.GetAsync(idCampania);
            switch ((EstadoProcesoType)campania.IdEstadoCampania)
            {
                case EstadoProcesoType.Borrador:
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                    new ServiceResponseError
                        {Message = "No se puede cancelar la planificación porque la campaña se encuentra en Borrador."}
                });
                case EstadoProcesoType.EnEjecucion:
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                    new ServiceResponseError
                        {Message = "No se puede cancelar la planificación porque la campaña se encuentra en Ejecución."}
                });
                case EstadoProcesoType.Finalizada:
                    return ServiceResponseFactory.CreateErrorResponse<bool>(new[]
                    {
                    new ServiceResponseError
                        {Message = "No se puede cancelar la planificación porque la campaña se encuentra Finalizada."}
                });
            }
            BackgroundJob.Delete(campania.IdScheduleGenerar);
            BackgroundJob.Delete(campania.IdScheduleObtener);
            campania.IdScheduleObtener = "";
            campania.IdScheduleGenerar = "";
            campania.IdEstadoCampania = 1;
            campania.FechaPlanificado = null;
            var userLogged = Convert.ToInt32(User.FindFirstValue("usrid") ?? "0");
            await _campaniaServices.UpdateAsync(campania, userLogged);
            return ServiceResponseFactory.CreateOkResponse(true);
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        [HttpGet]
        [Route("template")]
        [Authorize("notif_gestor_campanias")]
        [SwaggerOperation(Summary = "Notificaciones Digitales",
            Description = "Obtención Modelo Campanias Excel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Modelo Campanias Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Modelo Campanias Excel",
            typeof(IActionResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<IActionResult> DownloadExcelDemoAsync()
        {
            var response = await _campaniaServices.DownloadExcelDemoAsync();
            var memory = new MemoryStream(response.Result.FileStream) { Position = 0 };
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Result.Title);
        }

        [HttpPost]
        [Route("importCsvCampania")]
        [Authorize("notif_gestor_campanias")]
        public async Task<IActionResult> UploadCvsCampaniasAsync([FromForm(Name = "file")] IFormFile file)
        {
            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string fullPath = Path.Combine(Path.GetTempPath(), fileName);

                if (file.Length == 0)
                {
                    return BadRequest();
                }
                
                bool fileExists = (System.IO.File.Exists(fullPath) ? true : false);
                if (fileExists)
                {
                    System.IO.File.Delete(fullPath);
                }            

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);                                   
                }
                return Ok(new { fullPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("{idCampania}/getCampaniaHtml")]
        [Authorize("notif_gestor_campanias_formulario")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener Reporte Notificaciones Digitales",
            typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        //public async Task<ActionResult<ServiceResponse<CampaniaDto>>> GetCampaniaHtmlAsync([FromRoute] long idCampania)
        public async Task<ServiceResponse<CampaniaDto>> GetCampaniaHtmlAsync([FromRoute] long idCampania)
        {
            var response = await _campaniaServices.GetCampaniaHtmlAsync(idCampania);
            if (string.IsNullOrEmpty(response.TemplateName))
                return ServiceResponseFactory.CreateOkResponse(response);

            var mailgunRequest = new MailgunRequest
            {
                Tag = response.Tag,
                Description = response.Descripcion,
                Template = response.TemplateName,
                TemplateHtml = response.TemplateHtml
            };
            var templateMailgun = await _mailgun.GetTemplateAsync(TipoEnvioType.Campania, mailgunRequest, _configuration, _logProcess);
            if (templateMailgun.Template != null)
            {
                response.TemplateHtml = templateMailgun.Template.Version.Template;
            }
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        [HttpGet]
        [Route("{idComunicacion}/GetCampaniaHtmlOV")]
        [Authorize("notif_comunicaciones")]
        [SwaggerOperation(Summary = "Notificaciones Digitales", Description = "Obtención Notificaciones Digitales")]
        [SwaggerResponse(StatusCodes.Status200OK, "Devuelvo los eventos de una comunicación", typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al obtener los eventos de una comunicación", typeof(ServiceResponse<CampaniaDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autorizado")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Operación no permitida")]
        public async Task<ServiceResponse<CampaniaDto>> GetCampaniaHtmlOVAsync([FromRoute] long idComunicacion)
        {            
            var templateMailgun = new MailgunTemplateResponse();
            var response = await _comunicacionServices.GetRelEnvioAsync(idComunicacion);
            int idTipoEnvio = Convert.ToInt32(response.Envio.IdTipoEnvio);
            int idRelacionado = Convert.ToInt32(response.Envio.IdRelacionado);
            var campania = await _campaniaServices.GetAsync(idRelacionado);

            if (idTipoEnvio == 1)
            {//Campaña                
                if (string.IsNullOrEmpty(campania.TemplateName))
                    return ServiceResponseFactory.CreateOkResponse(campania);

                var mailgunRequest = new MailgunRequest
                {
                    Tag = campania.Tag,
                    Description = campania.Descripcion,
                    Template = campania.TemplateName,
                    TemplateHtml = campania.TemplateHtml
                };
                templateMailgun = await _mailgun.GetTemplateAsync(TipoEnvioType.Campania, mailgunRequest, _configuration, _logProcess);
            }
            else {
                //Negocio
                var tipoComunicacion = await _tipoComunicacionServices.GetAsync(idRelacionado);
                var mailgunRequest = new MailgunRequest
                {
                    Tag = tipoComunicacion.Tag,
                    Description = tipoComunicacion.Descripcion,
                    Template = tipoComunicacion.Template,
                    TemplateHtml = tipoComunicacion.TemplateHtml
                };
                templateMailgun = await _mailgun.GetTemplateAsync(TipoEnvioType.Negocio, mailgunRequest, _configuration, _logProcess);
            }
                        
            if (templateMailgun.Template != null)
            {
                campania.TemplateHtml = templateMailgun.Template.Version.Template;
            }

            return ServiceResponseFactory.CreateOkResponse(campania);            
        }

    }
}
