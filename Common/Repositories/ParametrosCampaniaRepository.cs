using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class ParametrosCampaniaRepository: GenericNotificacionRepository<ParametrosCampania>, IParametrosCampaniasRepository
    {
        public ParametrosCampaniaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
        }
    }
}
