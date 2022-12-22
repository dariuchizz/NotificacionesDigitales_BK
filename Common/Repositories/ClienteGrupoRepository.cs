using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class ClienteGrupoRepository : GenericNotificacionRepository<ClienteGrupoCategoria>, IClienteGrupoRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ClienteGrupoRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ComboGrupoCategoriasDto>> GetComboGrupoCategoriasAsync()
        {
            var temporal = await _context.ClienteGrupoCategorias
                .Include(i => i.GrupoCategorias)
                .Select(s => s).ToListAsync();

            var response = temporal
                        .GroupBy(g => g.GranCliente)
                        .Select(s => new ComboGrupoCategoriasDto
                        {
                            GranCliente = s.Key,
                            Descripcion = s.Key == true ? "Grandes Clientes" : "Residenciales + SGP",
                            DetalleGrupo = s.Select(g => new ComboAgrupacionStringDto
                            {
                                Agrupacion = g.Grupo,
                                Detalle = g.GrupoCategorias.Select(d => new ComboStringDto
                                {
                                    Id = $"{d.IdGrupoCategoria}-{d.Categoria}",
                                    Descripcion = d.Categoria
                                }).OrderBy(o=> o.Id)
                            }).OrderBy(o=> o.Agrupacion)
                        }).OrderBy(o=> o.Descripcion).ToList();
            return response;
        }

        public async Task<List<string>> GetGrupoCategoriasByCategoriasAsync(List<string> categorias)
        {
            var response = await _context.GrupoCategorias.Include(i => i.ClienteGrupoCategoria)
                .Where(w => categorias.Contains(w.Categoria))
                .Select(s => s.ClienteGrupoCategoria.Grupo).Distinct().ToListAsync();
            return response;
        }

        public async Task<List<string>> GetGrupoCategoriasByCategoriasAndGranClienteAsync(List<string> categorias, bool granCliente)
        {
            var response = await _context.GrupoCategorias
                .Include(i => i.ClienteGrupoCategoria)                
                .Where(w => categorias.Contains(w.Categoria))
                .Where(g => g.ClienteGrupoCategoria.GranCliente == granCliente)
                .Select(s => s.ClienteGrupoCategoria.Grupo).Distinct().ToListAsync();
            return response;
        }

        public async Task<List<string>> BuildCodesCategoriasByGroupAsync(List<string> codes)
        {
            var response = await _context.GrupoCategorias
                .Where(w => codes.Contains(w.Categoria))
                .Select(s => $"{s.IdGrupoCategoria}-{s.Categoria}")
                .ToListAsync();
            return response;
        }
    }
}
