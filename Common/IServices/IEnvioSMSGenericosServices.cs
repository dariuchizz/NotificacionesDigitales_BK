using Common.Model.Dto;
using Common.Model.Response;
using Common.Model.Services;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IEnvioSMSGenericosServices
    {
        Task<ServiceResponse<long>> AddEnvioSMSGenericoAsync(EnvioSMSRequest dto);
    }
}
