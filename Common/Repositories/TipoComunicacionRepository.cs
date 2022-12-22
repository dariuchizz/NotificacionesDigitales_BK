using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Common.Model.Response;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class TipoComunicacionRepository : GenericNotificacionRepository<TipoComunicacion>, ITipoComunicacionRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public TipoComunicacionRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TipoComunicacionesResponse>> GetAllWithEnviosAsync()
        {
            var enviosRealizados = _context.RptNotificaciones.Select(s => s.IdEnvio).Distinct();
            return await _context.TipoComunicacion
                .Include(e => e.Envios)
                .Select(s => new TipoComunicacionesResponse
            {
                Id = s.IdTipoComunicacion,
                Descripcion = s.Descripcion,
                Envios = s.Envios.Where(w => enviosRealizados.Contains(w.IdEnvio)).Select(e => e.IdEnvio)
            }).OrderBy(o => o.Descripcion).ToArrayAsync();
        }
    }
}
