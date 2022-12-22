using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class FacturaRepository: GenericNotificacionRepository<Factura>, IFacturaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public FacturaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Factura> GetAsync(string numeroFactura)
        {
            var factura = await _context.Factura.FirstOrDefaultAsync(f => f.NroFactura == numeroFactura);
            return factura;
        }
        public async Task<Factura> GetWithoutMaskAsync(string numeroFactura)
        {
            var factura = await _context.Factura.FirstOrDefaultAsync(f => f.NroFactura.Replace("-","").Replace("/","") == numeroFactura);
            return factura;
        }
        public async Task<Factura> GetAsync(long id)
        {
            var factura = await _context.Factura.FirstOrDefaultAsync(f => f.IdFactura == id);
            return factura;
        }
    }
}
