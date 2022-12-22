using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class EstadoCampaniasRepository: GenericNotificacionRepository<EstadoCampania>, IEstadoCampaniasRepository
    {
        public EstadoCampaniasRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
        }
    }
}
