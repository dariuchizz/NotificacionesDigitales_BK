using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Dapper;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class ProcesoEventoRepository : GenericNotificacionRepository<ProcesoEvento>, IProcesoEventoRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ProcesoEventoRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<long> AddEventProcessByStoreAsync(ProcesoEventoDto dto)
        { 
            var dynParams = new DynamicParameters();
            StringBuilder sql = new StringBuilder(1024);

            dynParams.Add($"@tipo", dto.Tipo);
            dynParams.Add($"@aviso", dto.Aviso);
            dynParams.Add($"@fecha", String.Format("{0:yyyy-MM-dd}", dto.Fecha));
            dynParams.Add($"@hora", dto.Hora);
            dynParams.Add($"@estado", dto.Estado);
            dynParams.Add($"@fechaUltimaModificacion", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dto.FechaUltimaModificacion));

            sql.Append("EXEC dbo.AgregarProcesoEvento ")
                .Append($"@tipo")
                .Append(",")
                .Append($"@aviso")
                .Append(",")
                .Append($"@fecha")
                .Append(",")
                .Append($"@hora")
                .Append(",")
                .Append($"@estado")
                .Append(",")
                .Append($"@fechaUltimaModificacion")
                .Append(";");

            var response = await _context.Connection().ExecuteScalarAsync<int>(sql.ToString(), dynParams, null, commandTimeout: 300);
            await _context.SaveChangesAsync();
            return response;
        }
    }
}
