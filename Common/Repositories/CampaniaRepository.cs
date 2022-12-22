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
    public class CampaniaRepository : GenericNotificacionRepository<Campania>, ICampaniaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CampaniaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CampaniaDto>> GridAsync(GridCampaniaRequest request)
        {
            try {
                var campania = string.IsNullOrEmpty(request.Campania) ? string.Empty : request.Campania.Trim().ToUpper();
                request.PageIndex++;
                var descending = request.Direction.ToLower().Trim() == "desc";
                var response = await _context.Campania
                    .Include(p => p.ParametrosCampanias)
                    .Include(c => c.ClaseCampania)
                    .Include(t => t.TipoCampania)
                    .Include(e => e.EstadoCampania)
                    .Where(w =>
                                (w.Descripcion.Trim().ToUpper().Contains(campania) || request.Campania == null) &&
                                (w.IdEstadoCampania == request.Estado || request.Estado == 0) &&
                                (w.IdClaseCampania == request.ClaseCampania || request.ClaseCampania == 0) &&
                                (w.FechaCreacion >= request.FechaDesde || request.FechaDesde == null) &&
                                (w.FechaCreacion <= request.FechaHasta || request.FechaHasta == null) &&
                                w.Activo)
                    .Select(s => new CampaniaDto
                    {
                        FechaCreacion = s.FechaCreacion,
                        Descripcion = s.Descripcion,
                        AutorModificacion = s.AutorModificacion,
                        FechaModificacion = s.FechaModificacion,
                        Activo = s.Activo,
                        TemplateName = s.Template,
                        Autor = s.Autor,
                        Asunto = s.Asunto,
                        CantidadEmails = s.CantidadEmails,
                        CantidadSuministros = s.CantidadSuministros,
                        FechaPlanificado = s.FechaPlanificado,
                        TipoCampania = new TipoCampaniaDto
                        {
                            StoreObtener = s.TipoCampania.StoreObtener,
                            StoreGenerar = s.TipoCampania.StoreGenerar,
                            IdTipoCampania = s.TipoCampania.IdTipoCampania,
                            StoreConsultar = s.TipoCampania.StoreConsultar,
                            TipoArmado = s.TipoCampania.TipoArmado
                        },
                        IdCampania = s.IdCampania,
                        IdCanalCampania = s.IdCanalCampania,
                        TopeLectura = s.TopeLectura,
                        Tag = s.Tag,
                        EstadoCampania = new EstadoCampaniaDto
                        {
                            IdEstadoCampania = s.EstadoCampania.IdEstadoCampania,
                            Descripcion = s.EstadoCampania.Descripcion
                        },
                        ClaseCampania = new ClaseCampaniaDto
                        {
                            IdClaseCampania = s.ClaseCampania.IdClaseCampania,
                            Descripcion = s.ClaseCampania.descripcion
                        },                        
                        IdClaseCampania = s.IdClaseCampania,
                    /*CsvCampanias = s.CsvCampanias.Select(c => new CsvCampaniaDto
                    {
                        IdCanal = c.IdCanal,
                        Dato = c.Dato,
                        IdCsvCampania = c.IdCsvCampania,
                    }),*/
                        ParametrosCampania = MapperParametros(s.ParametrosCampanias),
                        IdEstadoCampania = s.IdEstadoCampania
                    }).OrderByDynamic(request.Active, descending).ToPagedListAsync(request.PageIndex, request.PageSize);
                return response;
            } catch (Exception ex) { throw ex; }
            
        }

        public async Task<long> GridCounterAsync(GridCampaniaRequest request)
        {
            var campania = string.IsNullOrEmpty(request.Campania) ? string.Empty : request.Campania.Trim().ToUpper();
            request.PageIndex++;
            var descending = request.Direction.ToLower().Trim() == "desc";
            var response = await _context.Campania
                .Include(p => p.ParametrosCampanias)
                //.Include(c => c.CsvCampanias)
                .Include(t => t.TipoCampania)
                .Include(e => e.EstadoCampania)
                .CountAsync(
                    w => (w.Descripcion.Trim().ToUpper().Contains(campania) || request.Campania == null) &&
                                 (w.IdEstadoCampania == request.Estado || request.Estado == 0) &&
                                 (w.IdClaseCampania == request.ClaseCampania || request.ClaseCampania == 0) &&
                                 (w.FechaCreacion >= request.FechaDesde || request.FechaDesde == null) &&
                                 (w.FechaCreacion <= request.FechaHasta || request.FechaHasta == null) &&
                                 w.Activo);
            return response;
        }

        public async Task<IEnumerable<CampaniaDto>> ExcelAsync(GridCampaniaRequest request)
        {
            var campania = string.IsNullOrEmpty(request.Campania) ? string.Empty : request.Campania.Trim().ToUpper();
            var descending = request.Direction.ToLower().Trim() == "desc";
            var response = await _context.Campania
                .Include(p => p.ParametrosCampanias)
                //.Include(c => c.CsvCampanias)
                .Include(t => t.TipoCampania)
                .Include(e => e.EstadoCampania)
                .Where(w =>
                            (w.Descripcion.Trim().ToUpper().Contains(campania) || request.Campania == null) &&
                            (w.IdEstadoCampania == request.Estado || request.Estado == 0) &&
                            (w.IdClaseCampania == request.ClaseCampania || request.ClaseCampania == 0) &&
                            (w.FechaCreacion >= request.FechaDesde || request.FechaDesde == null) &&
                            (w.FechaCreacion <= request.FechaHasta || request.FechaHasta == null) &&
                            w.Activo)
                .Select(s => new CampaniaDto
                {
                    FechaCreacion = s.FechaCreacion,
                    Descripcion = s.Descripcion,
                    AutorModificacion = s.AutorModificacion,
                    FechaModificacion = s.FechaModificacion,
                    Activo = s.Activo,
                    TemplateName = s.Template,
                    Autor = s.Autor,
                    Asunto = s.Asunto,
                    CantidadSuministros = s.CantidadSuministros,
                    CantidadEmails = s.CantidadEmails,
                    FechaPlanificado = s.FechaPlanificado,
                    TipoCampania = new TipoCampaniaDto
                    {
                        StoreObtener = s.TipoCampania.StoreObtener,
                        StoreGenerar = s.TipoCampania.StoreGenerar,
                        IdTipoCampania = s.TipoCampania.IdTipoCampania,
                        StoreConsultar = s.TipoCampania.StoreConsultar,
                        TipoArmado = s.TipoCampania.TipoArmado
                    },
                    IdCampania = s.IdCampania,
                    TopeLectura = s.TopeLectura,
                    Tag = s.Tag,
                    EstadoCampania = new EstadoCampaniaDto
                    {
                        IdEstadoCampania = s.EstadoCampania.IdEstadoCampania,
                        Descripcion = s.EstadoCampania.Descripcion
                    },
                    ClaseCampania = new ClaseCampaniaDto
                    {
                        IdClaseCampania = s.ClaseCampania.IdClaseCampania,
                        Descripcion = s.ClaseCampania.descripcion
                    },
                    IdClaseCampania = s.IdClaseCampania,
                    /*CsvCampanias = s.CsvCampanias.Select(c => new CsvCampaniaDto
                    {
                        IdCanal = c.IdCanal,
                        Dato = c.Dato,
                        IdCsvCampania = c.IdCsvCampania,
                    }),*/
                    ParametrosCampania = MapperParametros(s.ParametrosCampanias),
                    IdEstadoCampania = s.IdEstadoCampania
                }).OrderByDynamic(request.Active, descending).ToListAsync();
            return response;
        }

        private static List<string> SplitString(string value)
        {
            return !string.IsNullOrEmpty(value) ? value.Split(',').ToList() : new List<string>();
        }

        private static ParametrosCampaniasDto MapperParametros(IEnumerable<ParametrosCampania> parametros)
        {
            var p = parametros.FirstOrDefault();
            if (p != null)
            {
                return new ParametrosCampaniasDto
                {
                    IdMotivoBaja = p.IdMotivoBaja,
                    TieneNotificacionDigital = BooleanToNumeric(p.TieneNotificacionDigital),
                    GranCliente = BooleanToNumeric(p.GranCliente),
                    TieneDebitoAutomatico = BooleanToNumeric(p.TieneDebitoAutomatico),
                    IdParametro = p.IdParametro,
                    Localidades = SplitString(p.Localidades),
                    EnteOficial = BooleanToNumeric(p.EnteOficial),
                    Categorias = SplitString(p.Categorias),
                    Estados = SplitString(p.Estados)
                };
            }

            return null;
        }

        private static int BooleanToNumeric(bool? boolean)
        {
            switch (boolean)
            {
                case null: return -1;
                case false: return 0;
                case true: return 1;
                default: return -1;
            }
        }

    }
}
