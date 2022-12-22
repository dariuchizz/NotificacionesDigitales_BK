using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class SuministroRepository : GenericNotificacionRepository<Suministro>, ISuministroRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public SuministroRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
