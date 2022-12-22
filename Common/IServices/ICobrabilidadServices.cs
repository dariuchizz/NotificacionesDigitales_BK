using System.Threading.Tasks;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface ICobrabilidadServices
    {
        Task<ServiceResponse<CobrabilidadResponse>> ReportAsync(CobrabilidadRequest request);
        Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(CobrabilidadRequest request);

    }
}
