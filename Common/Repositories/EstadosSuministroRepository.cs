using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class EstadosSuministroRepository: GenericNotificacionRepository<EstadosSuministro>, IEstadosSuministroRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EstadosSuministroRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
