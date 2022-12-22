using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class EnvioSMSGenericoRepository : GenericNotificacionRepository<EnvioSmsGenerico>, IEnvioSMSGenericoRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EnvioSMSGenericoRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
