using System.Collections.Generic;
using Common.Model.Dto;
using System.Threading.Tasks;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface ITipoComunicacionServices
    {
        Task<TipoComunicacionDto> GetAsync(int id);
        Task<ServiceResponse<IEnumerable<TipoComunicacionesResponse>>> GetAllAsync();
    }
}
