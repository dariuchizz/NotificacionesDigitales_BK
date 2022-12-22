using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class MotivoBajaRepository: GenericNotificacionRepository<MotivoBaja>, IMotivoBajaRepository
    {
        public MotivoBajaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
        }
    }
}
