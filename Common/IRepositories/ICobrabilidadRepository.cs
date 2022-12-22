using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Request;

namespace Common.IRepositories
{
    public interface ICobrabilidadRepository: IGenericRepository<RptNotificaciones>
    {
        Task<IEnumerable<NotificacionesDigitalesHaedDto>> ReportAsync(CobrabilidadRequest request);
        Task<long> ReportCountAsync(CobrabilidadRequest request);
        Task<IEnumerable<NotificacionesDigitalesHaedDto>> ExportExcelAsync(CobrabilidadRequest request);
    }
}
