using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class CambioContactoRepository : GenericNotificacionRepository<CambioContacto>, ICambioContactoRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CambioContactoRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
