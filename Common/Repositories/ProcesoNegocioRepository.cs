using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class ProcesoNegocioRepository : GenericNotificacionRepository<ProcesoNegocio>, IProcesoNegocioRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ProcesoNegocioRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
