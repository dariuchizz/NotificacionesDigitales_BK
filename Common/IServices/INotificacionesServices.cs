using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface INotificacionesServices
    {
        Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> NotificacionesAsync(
            ComunicacionType comunicacion, string numeroComprobante, int maxTake);

        Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> NotificacionesAsync(
            ComunicacionType comunicacion, long idComprobante, int maxTake);

        Task<ServiceResponse<IEnumerable<ReporteEventosPorCuentaDto>>> NotificacionesAsync(
            string cuentaUnificada, int mesesParaAtras);

        Task<ServiceResponse<NotifiacionCampaniaResponse>> NotificacionesCampaniaAsync(
            NotificacionRequest request);

        Task<ServiceResponse<NotifiacionProcesoNegocioResponse>> NotificacionesProcesoNegocioAsync(
            NotificacionRequest request);
    }
}
