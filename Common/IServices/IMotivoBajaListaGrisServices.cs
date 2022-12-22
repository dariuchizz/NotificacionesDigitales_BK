using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface IMotivoBajaListaGrisServices
    {
        Task<ServiceResponse<IEnumerable<MotivoBajaListaGrisResponse>>> GetAllAsync();
    }
}
