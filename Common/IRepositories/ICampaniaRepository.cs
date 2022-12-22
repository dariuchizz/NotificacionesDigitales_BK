using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Request;

namespace Common.IRepositories
{
    public interface ICampaniaRepository : IGenericRepository<Campania>
    {
        Task<IEnumerable<CampaniaDto>> GridAsync(GridCampaniaRequest request);
        Task<long> GridCounterAsync(GridCampaniaRequest request);
        Task<IEnumerable<CampaniaDto>> ExcelAsync(GridCampaniaRequest request);
    }
}
