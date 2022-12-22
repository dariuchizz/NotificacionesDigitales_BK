using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class CategoriasSuministroRepository: GenericNotificacionRepository<CategoriasSuministro>, ICategoriasSuministroRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CategoriasSuministroRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
