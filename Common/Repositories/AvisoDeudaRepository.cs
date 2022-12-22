using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class AvisoDeudaRepository: GenericNotificacionRepository<AvisosDeuda>, IAvisoDeudaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public AvisoDeudaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AvisosDeuda> GetAsync(long id)
        {
            var aviso = await _context.AvisosDeuda.FirstOrDefaultAsync(f => f.IdAvisoDeuda == id);
            return aviso;
        }
        public async Task<AvisosDeuda> GetAsync(string numeroComprobante)
        {
            var aviso = await _context.AvisosDeuda.FirstOrDefaultAsync(f => f.Numero == numeroComprobante);
            return aviso;
        }
    }
}
