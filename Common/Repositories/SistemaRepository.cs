using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class SistemaRepository : GenericNotificacionRepository<Sistema>, ISistemaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public SistemaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
