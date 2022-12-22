using System.Threading.Tasks;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface IAvisoDeudaRepository : IGenericRepository<AvisosDeuda>
    {
        Task<AvisosDeuda> GetAsync(long id);

        Task<AvisosDeuda> GetAsync(string numeroComprobante);
    }
}
