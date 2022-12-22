using System.Collections.Generic;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model.Request;

namespace Common.IRepositories
{
    public interface IComunicacionRepository : IGenericRepository<Comunicacion>
    {
        Task<ComunicacionDto> GetIdByMailgunAsync(string MessageId);

        Task<IEnumerable<Comunicacion>> GetErroresAsync();

        Task<IEnumerable<Comunicacion>> GetByTipoComunicacionIdRelacionadoAsync(ComunicacionType comunicacion,
            long idRelacionado);
        
        Task<IEnumerable<ReporteEventosPorCuentaDto>> ReportAsync(ReporteEventosPorCuentaRequest request);
        
        Task<long> ReportCounterAsync(ReporteEventosPorCuentaRequest request);
        
        Task<IEnumerable<ReporteEventosPorCuentaDto>> ExportExcelAsync(ReporteEventosPorCuentaRequest request);
        
        Task<IEnumerable<ReporteEventosPorCuentaDto>> GetComunicacionesCuentaUnificadaAsync(long idSuministro, int mesesParaAtras);
        
        Task<IEnumerable<NotificacionCampaniaDto>> NotificacionesCampaniaViewAsync(NotificacionRequest request, long IdComunicacion);

        Task<int> NotificacionesCampaniaViewCounterAsync(long IdComunicacion);

        Task<IEnumerable<NotificacionProcesoNegocioDto>> NotificacionesProcesoNegocioViewAsync(NotificacionRequest request, long IdComunicacion);

        Task<int> NotificacionesProcesoNegocioViewCounterAsync(long IdComunicacion);
    }
}
