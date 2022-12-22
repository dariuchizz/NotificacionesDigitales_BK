using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Request;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Common.Repositories
{
    public class EventosAccionesRepository: GenericNotificacionRepository<ConfiguracionEmail>, IEventosAccionesRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;
        public EventosAccionesRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventosAccionesDto>> GridAsync(GridEventosAccionesRequest request)
        {
            var desc = request.Direction.Trim().ToUpper() == "DESC";
            request.PageIndex = request.PageIndex + 1;
            var grid = await _context.ConfiguracionEmail
                .Select(s => new EventosAccionesDto
                {
                    Activo = s.Activo,
                    Id = s.IdConfiguracionEmail,
                    Codigo = s.Code,
                    CodigoRechazo = s.BounceCode,
                    Razon = s.Reason,
                    Severidad = s.Severity,
                    ResultanteRechazo = _context.EventosResultantesEmail.FirstOrDefault(f => f.IdEventoResultanteEmail == s.IdEventoResultanteEmail).Resultante
                }).OrderByDynamic(request.Active, desc)
                .ToPagedListAsync(request.PageIndex, request.PageIndex);
            return grid;
        }

        public async Task<long> CounterAsync(GridEventosAccionesRequest request)
        {
            var counter = await _context.ConfiguracionEmail.CountAsync();
            return counter;
        }

        public async Task<IEnumerable<EventosAccionesDto>> ExcelAsync(GridEventosAccionesRequest request)
        {
            var desc = request.Direction.Trim().ToUpper() == "DESC";
            var response = await _context.ConfiguracionEmail
                .Select(s => new EventosAccionesDto
                {
                    Activo = s.Activo,
                    Id = s.IdConfiguracionEmail,
                    Codigo = s.Code,
                    CodigoRechazo = s.BounceCode,
                    Razon = s.Reason,
                    Severidad = s.Severity,
                    ResultanteRechazo = _context.EventosResultantesEmail.FirstOrDefault().Resultante
                }).OrderByDynamic(request.Active, desc).ToArrayAsync();
            return response;
        }
    }
}
