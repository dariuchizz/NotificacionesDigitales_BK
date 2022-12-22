using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.NotificacionesDigitales;
using Common.Model.Response;

namespace Common.IRepositories
{
    public interface ITipoComunicacionRepository : IGenericRepository<TipoComunicacion>
    {
        Task<IEnumerable<TipoComunicacionesResponse>> GetAllWithEnviosAsync();
    }
}
