using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.Services
{
    public class NotificacionesServices : INotificacionesServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFacturaServices _facturaServices;
        private readonly IAvisoDeudaServices _avisoDeudaServices;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly IEventoEmailServices _eventoEmailServices;
        private readonly IEventoSMSServices _eventoSmsServices;

        public NotificacionesServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper,
            IFacturaServices facturaServices, IAvisoDeudaServices avisoDeudaServices,
            IComunicacionServices comunicacionServices, IEventoEmailServices eventoEmailServices,
            IEventoSMSServices eventoSmsServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _facturaServices = facturaServices;
            _avisoDeudaServices = avisoDeudaServices;
            _comunicacionServices = comunicacionServices;
            _eventoEmailServices = eventoEmailServices;
            _eventoSmsServices = eventoSmsServices;
        }

        public async Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> NotificacionesAsync(
            ComunicacionType comunicacion, string numeroComprobante, int maxTake)
        {
            numeroComprobante = Uri.UnescapeDataString(numeroComprobante);
            long idRelacionado;
            switch (comunicacion)
            {
                case ComunicacionType.VencimientoFacturaMail:
                case ComunicacionType.VencioTuFactura:
                case ComunicacionType.VencimientoFacturaSms:
                    FacturaDto factura;
                    if (!numeroComprobante.IsMatchRegexFactura())
                    {
                        return ReturnError<IEnumerable<EventoComunicacionDto>>("El comprobante ingresado no tiene el formato correcto.");
                    }
                    factura = await _facturaServices.GetAsync(numeroComprobante);
                    if (factura == null)
                    {
                        return ReturnError<IEnumerable<EventoComunicacionDto>>("No se encuentra el comprobante.");
                    }

                    idRelacionado = factura.IdFactura;
                    break;
                case ComunicacionType.AvisoDeCorte:
                    AvisoDeudaDto aviso;
                    if (!numeroComprobante.IsMatchRegexAvisoDeuda())
                    {
                        return ReturnError<IEnumerable<EventoComunicacionDto>>("El comprobante ingresado no tiene el formato correcto.");
                    }
                    aviso = await _avisoDeudaServices.GetAsync(numeroComprobante);
                    if (aviso == null)
                    {
                        return ReturnError<IEnumerable<EventoComunicacionDto>>("No se encuentra el comprobante.");
                    }

                    idRelacionado = aviso.IdAvisoDeuda;
                    break;
                default:
                    return ReturnError<IEnumerable<EventoComunicacionDto>>("El Tipo de Comunicacion ingresado no existe.");
            }

            return await NotificacionesAsync(comunicacion, idRelacionado, maxTake);
            //var comunicaciones =
            //    await _comunicacionServices.GetByTipoComunicacionIdRelacionadoAsync(comunicacion, idRelacionado);
            //var notificaciones = new List<NotificacionDto>();
            //if (comunicaciones != null)
            //{
            //    foreach (var c in comunicaciones)
            //    {
            //        var register = new NotificacionDto
            //        {
            //            Enviado = c.Enviado,
            //            Message = c.Message,
            //            AutorModificacion = c.AutorModificacion,
            //            FechaModificacion = c.FechaModificacion,
            //            IdTipoComunicacion = c.IdTipoComunicacion,
            //            IdComunicacion = c.IdComunicacion,
            //            Autor = c.Autor,
            //            FechaCreacion = c.FechaCreacion,
            //            Activo = c.Activo,
            //            IdContacto = c.IdContacto,
            //            IdRelacionado = c.IdRelacionado,
            //            IdExterno = c.IdExterno,
            //            FechaProceso = c.FechaProceso,
            //            IdCanal = c.IdCanal,
            //            Procesado = c.Procesado,
            //            Numero = c.IdCanal == 2 ? await GetNumeroCelularAsync(c.IdContacto) : "",
            //            Email = c.IdCanal == 1 ? await GetEmailAsync(c.IdContacto) : "",
            //            EstadoMail = c.IdCanal == 1 ? c.Procesado == 0 ? "PLANIFICADA" : await GetEstadoEmailAsync(c.IdComunicacion) : "",
            //            EstadoSms = c.IdCanal == 2 ? c.Procesado == 0 ? "PLANIFICADA" : await GetEstadoSmsAsync(c.IdComunicacion) : "",
            //        };
            //        notificaciones.Add(register);
            //    }
            //}
            //return ServiceResponseFactory.CreateOkResponse(result);
        }
        public async Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> NotificacionesAsync(
            ComunicacionType comunicacion, long idComprobante, int maxTake)
        {
            var response = await _unitOfWork.EventosComunicacionesRepository()
                .GetEventosByIdRelacionadoAsync(comunicacion, idComprobante, maxTake);
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<IEnumerable<ReporteEventosPorCuentaDto>>> NotificacionesAsync(
            string cuentaUnificada, int mesesParaAtras)
        {
            var suministro = await _unitOfWork.SuministroRepository()
                .FindByAsync(f => f.CuentaUnificada == Convert.ToInt64(cuentaUnificada));
            if (suministro == null)
            {
                return ReturnError<IEnumerable<ReporteEventosPorCuentaDto>>("La Cuenta Unificada no existe");
            }
            var response = await _unitOfWork.ComunicacionRepository()
                .GetComunicacionesCuentaUnificadaAsync(suministro.IdSuministro, mesesParaAtras);
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<NotifiacionCampaniaResponse>> NotificacionesCampaniaAsync(
            NotificacionRequest request)
        {
            var suministro = await _unitOfWork.SuministroRepository()
                .FindByAsync(f => f.CuentaUnificada == Convert.ToInt64(request.cuentaUnificada));
            if (suministro == null)
            {
                return ReturnError<NotifiacionCampaniaResponse>("La Cuenta Unificada no existe");
            }

            var response = new NotifiacionCampaniaResponse
            {
                Grid = await _unitOfWork.ComunicacionRepository().NotificacionesCampaniaViewAsync(request, suministro.IdSuministro),
                Cantidad = await _unitOfWork.ComunicacionRepository().NotificacionesCampaniaViewCounterAsync(suministro.IdSuministro)
            };
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<NotifiacionProcesoNegocioResponse>> NotificacionesProcesoNegocioAsync(
            NotificacionRequest request)
        {
            var suministro = await _unitOfWork.SuministroRepository()
                .FindByAsync(f => f.CuentaUnificada == Convert.ToInt64(request.cuentaUnificada));
            if (suministro == null)
            {
                return ReturnError<NotifiacionProcesoNegocioResponse>("La Cuenta Unificada no existe");
            }

            var response = new NotifiacionProcesoNegocioResponse
            {
                Grid = await _unitOfWork.ComunicacionRepository().NotificacionesProcesoNegocioViewAsync(request, suministro.IdSuministro),
                Cantidad = await _unitOfWork.ComunicacionRepository().NotificacionesProcesoNegocioViewCounterAsync(suministro.IdSuministro)
            };
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        private ServiceResponse<T> ReturnError<T>(string message)
        {
            return ServiceResponseFactory.CreateErrorResponse<T>(new[]
            {
                new ServiceResponseError
                    {Message = message}
            });
        }


        private async Task<string> GetNumeroCelularAsync(long idContacto)
        {
            var celular = await _unitOfWork.CelularRepository().FindByAsync(f => f.IdCelular == idContacto);
            return celular != null ? celular.Numero : "";
        }

        private async Task<string> GetEmailAsync(long idContacto)
        {
            var email = await _unitOfWork.EmailRepository().FindByAsync(f => f.IdEmail == idContacto);
            return email != null ? email.DEmail : "";
        }

        private async Task<string> GetEstadoSmsAsync(long idComunicacion)
        {
            var evento = await _unitOfWork.EventoSMSRepository().GetLastStateByIdComunicacionAsync(idComunicacion);
            return evento;
        }

        private async Task<string> GetEstadoEmailAsync(long idComunicacion)
        {
            var evento = await _unitOfWork.EventoEmailRepository().GetLastEventByIdComunicacionAsync(idComunicacion);
            return evento;
        }

    }
}
