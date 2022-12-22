using Common.IRepositories;
using Common.Model.NotificacionesDigitales;

namespace Common.Repositories
{
    public class EventosResultantesEmailsRepository : GenericNotificacionRepository<EventosResultantesEmail>, IEventosResultantesEmailRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EventosResultantesEmailsRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<ComboSingleDto>> GetDataComboAsync()
        //{
        //    var response = await _context.EventosResultantesEmail
        //        .Select(s => new ComboSingleDto
        //        {
        //            Id = s.IdEventoResultanteEmail,
        //            Descripcion = s.Resultante
        //        }).ToListAsync();
        //    return response;
        //}
    }
}
