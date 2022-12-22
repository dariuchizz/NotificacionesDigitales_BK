using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Dapper;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class ListaGrisRepository : GenericNotificacionRepository<ListaGris>, IListaGrisRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ListaGrisRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> AddByStoreAsync(UnsuscribeDto unsuscribeDto)
        {
            var dynParams = new DynamicParameters();
            StringBuilder sql = new StringBuilder(1024);

            dynParams.Add($"@idComunicacion", unsuscribeDto.IdComunicacion);
            dynParams.Add($"@guid", unsuscribeDto.GUID);
            dynParams.Add($"@idMotivo", unsuscribeDto.IdMotivo);
            dynParams.Add($"@observacionCliente", unsuscribeDto.ObservacionCliente);
            dynParams.Add($"@origen", unsuscribeDto.Origen);            

            sql.Append("EXEC dbo.AgregarListaGris ")
            .Append($"@idComunicacion")
            .Append(",")
            .Append($"@guid")
            .Append(",")
            .Append($"@idMotivo")
            .Append(",")
            .Append($"@observacionCliente")
            .Append(",")
            .Append($"@origen")
            .Append(";");

            await _context.Connection().ExecuteAsync(sql.ToString(), dynParams, null, commandTimeout: 300);
            return await _context.SaveChangesAsync();
        }
    }
}
