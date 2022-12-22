using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;
using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class EventosComunicacionesRepository : GenericNotificacionRepository<EventosComunicaciones>, IEventosComunicacionesRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EventosComunicacionesRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventoComunicacionDto>> GetEventosByIdRelacionadoAsync(ComunicacionType comunicacion,
            long idRelacionado, int maxTake)
        {
            var response = await _context.EventosComunicaciones
                .Include(i => i.Comunicacion)
                .Include(i => i.EventosResultante)
                .Join(_context.Envios.Include(e=> e.TipoEnvio), ec=> ec.Comunicacion.IdEnvio, e=> e.IdEnvio, (ec, e)=> new {ec, e})
                .Where(w => w.e.IdRelacionado == (int)comunicacion &&
                            w.ec.Comunicacion.IdRelacionado == idRelacionado && w.e.IdTipoEnvio == 2)
                .Select(s => new EventoComunicacionDto
                {
                    FechaEvento = s.ec.FechaCreacion,
                    Nombre = s.ec.EventosResultante.Nombre,
                    Contacto = s.ec.Comunicacion.IdCanal == 1 ?
                            _context.Email.Any(a => a.IdEmail == s.ec.Comunicacion.IdContacto) ?
                                _context.Email.First(f => f.IdEmail == s.ec.Comunicacion.IdContacto).DEmail : ""
                            : _context.Celular.Any(a=> a.IdCelular == s.ec.Comunicacion.IdContacto) ?
                                _context.Celular.First(f=> f.IdCelular == s.ec.Comunicacion.IdContacto).Numero : ""
                })
                .Take(maxTake).ToArrayAsync();
            return response;
        }

        public async Task<IEnumerable<EventoComunicacionDto>> GetEventosByIdComunicacionAsync(long idComunicacion)
        {
            var response = await _context.EventosComunicaciones
                .Include(i => i.EventosResultante)
                .Where(w => w.IdComunicacion == idComunicacion)
                .Select(s => new EventoComunicacionDto
                {
                    FechaEvento = s.FechaEvento,
                    Nombre = s.EventosResultante.Nombre
                }).OrderBy(o => "FechaEvento ASC").ToArrayAsync();
            return response;
        }
    }
}
