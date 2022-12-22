using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class TipoCampaniaRepository: GenericNotificacionRepository<TipoCampania>, ITipoCampaniaRepository
    {
        public TipoCampaniaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
        }
    }
}
