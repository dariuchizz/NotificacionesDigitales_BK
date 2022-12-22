using Common.IRepositories;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class EventoEmailRepository : GenericNotificacionRepository<EventoEmail>, IEventoEmailRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EventoEmailRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<DateTime?> GetLastDateEventAsync()
        {
            var fecha = _context.EventoEmail
                .Select(s => s).OrderByDescending(o => o.Fecha).FirstOrDefault()?.Fecha;

            return Task.FromResult(fecha);
        }

        public async Task<int> AddEventsEmailsByStoreAsync(List<EventoEmailDto> dto)
        {
            var dynParams = new DynamicParameters();
            StringBuilder sql = new StringBuilder(1024);

            int i = 0;
            foreach (var item in dto)
            {
                dynParams.Add($"@fecha{i}", String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.Fecha));
                dynParams.Add($"@evento{i}", item.DEvento);
                dynParams.Add($"@idEvento{i}", item.IdEvento);
                dynParams.Add($"@idExterno{i}", item.IdExterno);
                dynParams.Add($"@message{i}", item.Message);
                dynParams.Add($"@reason{i}", item.Reason);
                dynParams.Add($"@code{i}", item.Code);
                dynParams.Add($"@bounceCode{i}", item.BounceCode);
                dynParams.Add($"@severity{i}", item.Severity);
                dynParams.Add($"@messageError{i}", item.MessageError);

                sql.Append("EXEC dbo.AgregarEventoEmail ")
                     .Append($"@fecha{i}")
                     .Append(",")
                     .Append($"@evento{i}")
                     .Append(",")
                     .Append($"@idEvento{i}")
                     .Append(",")
                     .Append($"@idExterno{i}")
                     .Append(",")
                     .Append($"@message{i}")
                     .Append(",")
                     .Append($"@reason{i}")
                     .Append(",")
                     .Append($"@code{i}")
                     .Append(",")
                     .Append($"@bounceCode{i}")
                     .Append(",")
                     .Append($"@severity{i}")
                     .Append(",")
                     .Append($"@messageError{i}")
                     .Append(";");
                i++;
            }

            await _context.Connection().ExecuteAsync(sql.ToString(), dynParams, null, commandTimeout: 300);
            return await _context.SaveChangesAsync();
        }

        public async Task<string> GetLastEventByIdComunicacionAsync(long idComunicacion)
        {
            var evento = await _context.EventoEmail
                .Select(s => s).Where(w => w.IdComunicacion == idComunicacion).OrderByDescending(o => o.Fecha).ThenByDescending(o => o.IdEventoEmail).FirstOrDefaultAsync();
            if (evento == null) return "ENVIADA";
            switch (evento.DEvento.ToUpper().Trim())
            {
                case "ACCEPTED":
                    return "ENVIADA";
                case "DELIVERED":
                case "OPENED":
                case "CLICKED":
                case "COMPLAINED":
                    return "RECIBIDO";
                case "FAILED":
                    return "FALLADO";
                default:
                    return "";
            }
        }
    }
}
