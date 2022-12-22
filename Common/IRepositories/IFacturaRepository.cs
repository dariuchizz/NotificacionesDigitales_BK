using System.Threading.Tasks;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface IFacturaRepository : IGenericRepository<Factura>
    {
        Task<Factura> GetAsync(string numeroFactura);

        Task<Factura> GetWithoutMaskAsync(string numeroFactura);

        Task<Factura> GetAsync(long id);
    }
}
