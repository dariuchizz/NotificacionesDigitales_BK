using Common.Model.Enum;
using Common.Model.NotificacionesDigitales;
using System;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IProcesoEventoServices
    {
        Task<long> AddEventProcessByStoreAsync(ProcesoEventoDto dto);

        Task<long> AddProcesoEventoAsync(ProcesoEventoDto dto);

        Task UpdateProcesoEventoAsync(ProcesoEventoDto dto);

        Task<ProcesoEventoDto> GetAsync(long id);

        Task<ProcesoEventoDto> GetAsync(DateTime fechaAProcesar, TipoProcesoEvento tipoProcesoEvento, bool? Aviso);
        }
}
