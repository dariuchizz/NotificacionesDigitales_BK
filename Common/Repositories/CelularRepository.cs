using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class CelularRepository: GenericNotificacionRepository<Celular>, ICelularRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CelularRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
