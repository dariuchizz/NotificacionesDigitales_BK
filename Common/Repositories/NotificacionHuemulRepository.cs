using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Common.Repositories
{
    public class NotificacionHuemulRepository : GenericNotificacionRepository<NotificacionHuemul>, INotificacionHuemulRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public NotificacionHuemulRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> AddNotificacionesHuemulByStoreAsync(List<NotificacionHuemulDto> dto)
        {
            var dynParams = new DynamicParameters();
            StringBuilder sql = new StringBuilder(1024);

            int i = 0;
            foreach (var item in dto)
            {
                dynParams.Add($"@cuentaUnificada{i}", item.CuentaUnificada);
                dynParams.Add($"@tipoComprobante{i}", item.TipoComprobante);
                dynParams.Add($"@nroComprobante{i}", item.NroComprobante);
                dynParams.Add($"@canal{i}", item.Canal);
                dynParams.Add($"@link{i}", item.LinkHuemul);

                sql.Append("EXEC dbo.AgregarNotificacionesHuemul ")
                     .Append($"@cuentaUnificada{i}")
                     .Append(",")
                     .Append($"@tipoComprobante{i}")
                     .Append(",")
                     .Append($"@nroComprobante{i}")
                     .Append(",")
                     .Append($"@canal{i}")
                     .Append(",")
                     .Append($"@link{i}")
                     .Append(";");
                i++;
            }
            await _context.Connection().ExecuteAsync(sql.ToString(), dynParams, null, commandTimeout: 300);
            return await _context.SaveChangesAsync();
        }
    }
}