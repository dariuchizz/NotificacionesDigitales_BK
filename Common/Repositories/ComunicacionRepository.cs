using System;
using System.Collections.Generic;
using System.Linq;
using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model.Enum;
using Common.Model.Request;
using X.PagedList;

namespace Common.Repositories
{
    public class ComunicacionRepository : GenericNotificacionRepository<Comunicacion>, IComunicacionRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ComunicacionRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ComunicacionDto> GetIdByMailgunAsync(string MessageId)
        {
            var comunicacion = await _context.Comunicacion.FirstOrDefaultAsync(f => f.IdExterno == MessageId);
            if (comunicacion == null)
            {
                return null;
            }

            var response = new ComunicacionDto
            {
                IdComunicacion = comunicacion.IdComunicacion,
                IdTipoComunicacion = _context.Envios.Single(s=> s.IdEnvio == comunicacion.IdEnvio).IdRelacionado,
                IdCanal = comunicacion.IdCanal,
                IdRelacionado = comunicacion.IdRelacionado,
                IdContacto = comunicacion.IdContacto,
                FechaProceso = comunicacion.FechaProceso,
                IdExterno = comunicacion.IdExterno,
                Message = comunicacion.Message
            };
            return response;
        }

        public async Task<IEnumerable<Comunicacion>> GetErroresAsync()
        {
            var date = DateTime.Now.AddDays(-1);
            var comunicaciones = await _context.Comunicacion.Where(w =>
                w.FechaProceso >= date && w.Enviado == (int) EstadoComunicacion.Error).ToArrayAsync();

            return comunicaciones;
        }

        public async Task<IEnumerable<Comunicacion>> GetByTipoComunicacionIdRelacionadoAsync(
            ComunicacionType comunicacion, long idRelacionado)
        {
            var idTipoComunicacion = (int) comunicacion;
            var response = await _context.Comunicacion
                .Join(_context.Envios, comunicacion1 => comunicacion1.IdEnvio, e=> e.IdEnvio, (c, e)=> new {c,e})
                .Where(w =>
                w.e.IdRelacionado == idTipoComunicacion && w.c.IdRelacionado == idRelacionado).Select(s=> s.c).ToArrayAsync();
            return response;
        }

        public async Task<IEnumerable<ReporteEventosPorCuentaDto>> ReportAsync(ReporteEventosPorCuentaRequest request)
        {
            request.PageIndex++;
            var response = await BuildSql(request)
                .ToPagedListAsync(request.PageIndex, request.PageSize);
            return response;
        }

        public async Task<long> ReportCounterAsync(ReporteEventosPorCuentaRequest request)
        {
            var response = await BuildSql(request).CountAsync();
            return response;
        }

        public async Task<IEnumerable<ReporteEventosPorCuentaDto>> ExportExcelAsync(ReporteEventosPorCuentaRequest request)
        {
            var response = await BuildSql(request).ToArrayAsync();
            return response;
        }

        private IQueryable<ReporteEventosPorCuentaDto> BuildSql(ReporteEventosPorCuentaRequest request)
        {
            var descending = request.Direction.Trim().ToUpper() == "DESC";
            var telefonos = _context.Celular.Where(w => w.IdSuministro == request.IdSuministro).Select(s=> s.IdCelular);
            var emails = _context.Email.Where(w => w.IdSuministro == request.IdSuministro).Select(s=> s.IdEmail);

            var dateToStart = Convert.ToDateTime(DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy"));

            var result = _context.Comunicacion
                .Include(ec => ec.EventosComunicaciones)
                //.Include(e=> e.Envio)
                //.ThenInclude(t=> t.TipoComunicacion)
                .Where(w => w.IdCanal == 1 && emails.Contains(w.IdContacto) && w.FechaCreacion >= dateToStart)
                .Union(_context.Comunicacion
                    .Where(w => w.IdCanal == 2 && telefonos.Contains(w.IdContacto) && w.FechaCreacion >= dateToStart))
                .Select(c => new ReporteEventosPorCuentaDto
                {
                    IdRelacionado = c.IdRelacionado,
                    Contacto = c.IdCanal == 1 ?
                        _context.Email.Any(f => f.IdEmail == c.IdContacto)
                            ? _context.Email.FirstOrDefault(f => f.IdEmail == c.IdContacto).DEmail
                            : "" :
                        _context.Celular.Any(f => f.IdCelular == c.IdContacto) ? _context.Celular.FirstOrDefault(f => f.IdCelular == c.IdContacto).Numero : "",
                    IdComunicacion = c.IdComunicacion,
                    IdCanal = c.IdCanal,
                    IdTipoComunicacion = _context.Envios.Single(s=> s.IdEnvio == c.IdEnvio).IdRelacionado,
                    TipoComunicacion = _context.Envios.Where(e => e.IdEnvio == c.IdEnvio)
                        .Select(e=> e.IdTipoEnvio == 2 ? 
                            _context.TipoComunicacion.Single(s=> s.IdTipoComunicacion == e.IdRelacionado).Descripcion :
                            _context.Campania.Single(c=> c.IdCampania == e.IdRelacionado).Descripcion).First(),
                    IdContacto = c.IdContacto,
                    Canal = c.Canale.Descripcion,
                    IdEnvio = c.IdEnvio.GetValueOrDefault(0),
                    FechaEnvio = c.FechaProceso,
                    Eventos = c.EventosComunicaciones.Select(e => new EventoComunicacionDto
                    {
                        Nombre = e.EventosResultante.Descripcion,
                        FechaEvento = e.FechaCreacion,
                        Contacto = e.Comunicacion.IdCanal == 1 ?
                            _context.Email.Any(a => a.IdEmail == e.Comunicacion.IdContacto) ?
                                _context.Email.First(f => f.IdEmail == e.Comunicacion.IdContacto).DEmail : ""
                            : _context.Celular.Any(a => a.IdCelular == e.Comunicacion.IdContacto) ?
                                _context.Celular.First(f => f.IdCelular == e.Comunicacion.IdContacto).Numero : ""
                    })
                });
                //.OrderByDynamic(request.Active, descending);
            return result;
        }

        public async Task<IEnumerable<ReporteEventosPorCuentaDto>> GetComunicacionesCuentaUnificadaAsync(long idSuministro, int mesesParaAtras)
        {
            var telefonos = await _context.Celular.Where(w => w.IdSuministro == idSuministro).Select(s => s.IdCelular).ToArrayAsync();
            var emails = await _context.Email.Where(w => w.IdSuministro == idSuministro).Select(s => s.IdEmail).ToArrayAsync();

            var dateToStart = Convert.ToDateTime(DateTime.Now.AddMonths(-mesesParaAtras).ToString("dd/MM/yyyy"));

            var result = await _context.Comunicacion
                .Where(w => w.IdCanal == 1 && emails.Contains(w.IdContacto) && w.FechaCreacion >= dateToStart)
                .Union(_context.Comunicacion
                    .Where(w => w.IdCanal == 2 && telefonos.Contains(w.IdContacto) && w.FechaCreacion >= dateToStart))
                .Select(c => new ReporteEventosPorCuentaDto
                {
                    IdComunicacion = c.IdComunicacion,
                    IdEnvio = c.IdEnvio.GetValueOrDefault(0),
                    Descripcion = _context.Envios.Where(e => e.IdEnvio == c.IdEnvio)
                        .Select(e => e.IdTipoEnvio == 2 ?
                            _context.TipoComunicacion.Single(s => s.IdTipoComunicacion == e.IdRelacionado).Descripcion :
                            _context.Campania.Single(c => c.IdCampania == e.IdRelacionado).Descripcion).First(),
                    TipoComunicacion = 
                        _context.Envios.Include(t=> t.TipoEnvio)
                            .Single(s=> s.IdRelacionado == c.IdRelacionado).TipoEnvio.Dominio,
                    Canal = c.Canale.Descripcion,
                    Contacto = c.IdCanal == 1 ?
                        _context.Email.Any(f => f.IdEmail == c.IdContacto)
                            ? _context.Email.FirstOrDefault(f => f.IdEmail == c.IdContacto).DEmail
                            : "" :
                        _context.Celular.Any(f => f.IdCelular == c.IdContacto) ? _context.Celular.FirstOrDefault(f => f.IdCelular == c.IdContacto).Numero : "",
                    FechaEnvio = c.FechaProceso,
                    Eventos = c.EventosComunicaciones.Select(e => new EventoComunicacionDto
                    {
                        Nombre =_context.EventosResultantes.Single(s=> s.IdEventoResultante == e.IdEventoResultante).Nombre,
                        FechaEvento = e.FechaCreacion,
                        Contacto = e.Comunicacion.IdCanal == 1 ?
                            _context.Email.Any(a => a.IdEmail == e.Comunicacion.IdContacto) ?
                                _context.Email.First(f => f.IdEmail == e.Comunicacion.IdContacto).DEmail : ""
                            : _context.Celular.Any(a => a.IdCelular == e.Comunicacion.IdContacto) ?
                                _context.Celular.First(f => f.IdCelular == e.Comunicacion.IdContacto).Numero : ""
                    })
                }).ToArrayAsync();
            return result;
        }

        public async Task<IEnumerable<NotificacionCampaniaDto>> NotificacionesCampaniaViewAsync(NotificacionRequest request, long IdSuministro)
        {
            var descending = true;
            if (string.IsNullOrEmpty(request.Direction))
                descending = true;
            else
                descending = request.Direction.ToLower().Trim() == "desc";

            if (string.IsNullOrEmpty(request.Active))
                request.Active = "FechaEnvio";

            if (string.IsNullOrEmpty(request.Active))
                request.Active = "FechaEnvio";

            request.PageIndex += 1;
            var ordering = $"{request.Active} {request.Direction}";

            try
            {
                var response = await _context.v_ObtenerSuministroCampanias
            .Select(s => new NotificacionCampaniaDto
            {
                IdComunicacion = s.IdComunicacion,
                IdSuministro = s.IdSuministro,
                Descripcion = s.Descripcion,
                FechaEnvio = s.FechaEnvio,
                IdCanalCampania = s.IdCanalCampania,
                Canal = s.Canal,
                Contacto = s.Contacto,
                
            })
            .Where(w => w.IdSuministro == IdSuministro)
            .OrderByDynamic(request.Active, descending)
            .ToPagedListAsync(request.PageIndex, request.PageSize);

                return response;
            }
            catch (Exception ec) { throw ec; }
            
        }

        public async Task<int> NotificacionesCampaniaViewCounterAsync(long IdSuministro)
        {
            var response = await _context.v_ObtenerSuministroCampanias
               .CountAsync(w => w.IdSuministro == IdSuministro);
            
            return response;
        }

        public async Task<IEnumerable<NotificacionProcesoNegocioDto>> NotificacionesProcesoNegocioViewAsync(NotificacionRequest request, long IdSuministro)
        {
            var descending = true;
            if (string.IsNullOrEmpty(request.Direction))
                descending = true;
            else
                descending = request.Direction.ToLower().Trim() == "desc";

            if (string.IsNullOrEmpty(request.Active))
                request.Active = "FechaEnvio";

            request.PageIndex += 1;
            var ordering = $"{request.Active} {request.Direction}";

            var response = await _context.v_ObtenerSuministroProcesosNegocios
            .Select(s => new NotificacionProcesoNegocioDto
            {
                IdComunicacion = s.IdComunicacion,
                IdSuministro = s.IdSuministro,
                Descripcion = s.Descripcion,
                FechaEnvio = s.FechaEnvio,
                Canal = s.Canal,
                Contacto = s.Contacto,
                Dato = s.Dato,
                IdCanal = s.IdCanal,
            })
            .Where(w => w.IdSuministro == IdSuministro)
            .OrderByDynamic(request.Active, descending)
            .ToPagedListAsync(request.PageIndex, request.PageSize);
            
            return response;
        }

        public async Task<int> NotificacionesProcesoNegocioViewCounterAsync(long IdSuministro)
        {
            var response = await _context.v_ObtenerSuministroProcesosNegocios
               .CountAsync(w => w.IdSuministro == IdSuministro);

            return response;
        }
    }
}
