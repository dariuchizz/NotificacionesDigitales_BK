using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface IProcesoEventoRepository : IGenericRepository<ProcesoEvento>
    {
        Task<long> AddEventProcessByStoreAsync(ProcesoEventoDto dto);
    }
}
