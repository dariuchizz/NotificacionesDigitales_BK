using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class MotivoBajaListaGrisRepository : GenericNotificacionRepository<MotivoBajaListaGris>, IMotivoBajaListaGrisRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public MotivoBajaListaGrisRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
