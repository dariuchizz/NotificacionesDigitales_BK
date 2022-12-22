using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface IEventosAccionesServices
    {
        Task<ServiceResponse<GridEventosAccionesResponse>> GridAsync(GridEventosAccionesRequest request);
        Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(GridEventosAccionesRequest request);
        Task<ServiceResponse<EventoAccionesFormularioDto>> GetAsync(long id);
        Task<ServiceResponse<bool>> PutAsync(long id, EventoAccionesFormularioDto item);
    }
}
