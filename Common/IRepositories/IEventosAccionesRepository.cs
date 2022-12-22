using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Request;

namespace Common.IRepositories
{
    public interface IEventosAccionesRepository : IGenericRepository<ConfiguracionEmail>
    {
        Task<IEnumerable<EventosAccionesDto>> GridAsync(GridEventosAccionesRequest request);
        Task<long> CounterAsync(GridEventosAccionesRequest request);
        Task<IEnumerable<EventosAccionesDto>> ExcelAsync(GridEventosAccionesRequest request);
    }
}
