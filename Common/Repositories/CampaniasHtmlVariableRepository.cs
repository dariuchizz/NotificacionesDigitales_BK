using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class CampaniasHtmlVariableRepository: GenericNotificacionRepository<CampaniasHtmlVariable>,ICampaniasHtmlVariableRepository
    {
        public CampaniasHtmlVariableRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
        }
    }
}
