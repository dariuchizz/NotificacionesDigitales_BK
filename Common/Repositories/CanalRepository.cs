using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class CanalRepository: GenericNotificacionRepository<Canal>, ICanalRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CanalRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
