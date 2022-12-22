using System;
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
    public class CobrabilidadRepository: GenericNotificacionRepository <RptNotificaciones>, ICobrabilidadRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CobrabilidadRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<NotificacionesDigitalesHaedDto>> ReportAsync(CobrabilidadRequest request)
        {
            var descending = request.Direction.Trim().ToUpper() != "DESC";
            request.PageIndex = request.PageIndex + 1;
            var busqueda = await BuildFilter(request)
                .Include(i => i.RptNotificacionesDetalles)
                .OrderByDynamic(request.Active, descending).ToPagedListAsync(request.PageIndex, request.PageSize);

            var response = busqueda.Select(s => new NotificacionesDigitalesHaedDto
            {
                IdTipoComunicacion = s.IdTipoComunicacion,
                IdEnvio = s.IdEnvio,
                CantidadFacturas = s.CantidadFacturas,
                CantidadNotificaciones = s.CantidadNotificaciones,
                FechaEnvio = s.FechaEnvio,
                FechaVencimiento = s.FechaVencimiento,
                TotalNotificado = s.TotalNotificado,
                Detalle = s.RptNotificacionesDetalles.Select(d => new NotificacionesDigitalesDetalleDto
                {
                    CantidadFacturas = d.CantidadFacturas,
                    IdTipoComunicacion = d.IdTipoComunicacion,
                    IdEnvio = d.IdEnvio,
                    CantidadDias = d.CantidadDias,
                    FechaPago = d.FechaPago,
                    PorcentajeCobrado = d.PorcentajeCobrado,
                    PorcentajeRecaudado = d.PorcentajeRecaudado,
                    Saldo = d.Saldo,
                    TotalRecaudado = d.TotalRecaudado,
                    Totales = d.Totales
                }).Where(w => w.CantidadDias <= 10)
            });

            return response;
        }
        public async Task<long> ReportCountAsync(CobrabilidadRequest request)
        {
            var response = await BuildFilter(request)
                .LongCountAsync();
            return response;
        }
        public async Task<IEnumerable<NotificacionesDigitalesHaedDto>> ExportExcelAsync(CobrabilidadRequest request)
        {
            var descending = request.Direction.Trim().ToUpper() == "DESC";
            var response = await BuildFilter(request)
                .Select(s => new NotificacionesDigitalesHaedDto
                {
                    IdTipoComunicacion = s.IdTipoComunicacion,
                    IdEnvio = s.IdEnvio,
                    CantidadFacturas = s.CantidadFacturas,
                    CantidadNotificaciones = s.CantidadNotificaciones,
                    FechaEnvio = s.FechaEnvio,
                    FechaVencimiento = s.FechaVencimiento,
                    TotalNotificado = s.TotalNotificado,
                    Detalle = s.RptNotificacionesDetalles.Select(d => new NotificacionesDigitalesDetalleDto
                    {
                        CantidadFacturas = d.CantidadFacturas,
                        IdTipoComunicacion = d.IdTipoComunicacion,
                        IdEnvio = d.IdEnvio,
                        CantidadDias = d.CantidadDias,
                        FechaPago = d.FechaPago,
                        PorcentajeCobrado = d.PorcentajeCobrado,
                        PorcentajeRecaudado = d.PorcentajeRecaudado,
                        Saldo = d.Saldo,
                        TotalRecaudado = d.TotalRecaudado,
                        Totales = d.Totales
                    })
                }).OrderByDynamic(request.Active, descending).ToArrayAsync();

            return response;
        }
        private IQueryable<RptNotificaciones> BuildFilter(CobrabilidadRequest request)
        {
            var envios = new List<long>();
            if (!string.IsNullOrEmpty(request.IdEnvios))
            {
                var split = request.IdEnvios.Split(',');
                foreach (var s in split)
                {
                    if (Convert.ToInt64(s) == 0)
                    {
                        continue;
                    }
                    envios.Add(Convert.ToInt64(s));
                }
            }
            return _context.RptNotificaciones
                .Where(w =>
                    ((w.FechaEnvio >= request.DateFrom && w.FechaEnvio <= request.DateTo) ||
                     (w.FechaEnvio >= request.DateFrom && request.DateTo == null) ||
                     (request.DateFrom == null && w.FechaEnvio <= request.DateTo) ||
                     (request.DateFrom == null && request.DateTo == null)) &&
                    (envios.Contains(w.IdEnvio) || envios.Count == 0) &&
                    (w.IdTipoComunicacion == request.TipoComunicacion || request.TipoComunicacion == 0));
        }

    }
}
