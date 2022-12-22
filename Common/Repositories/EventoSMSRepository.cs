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
    public class EventoSMSRepository : GenericNotificacionRepository<EventoSMS>, IEventoSMSRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EventoSMSRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<DateTime?> GetLastDateEventAsync()
        {
            var fecha = _context.EventoSMS
                .Select(s => s).OrderByDescending(o => o.Fecha).FirstOrDefault()?.Fecha;

            return Task.FromResult(fecha);
        }

        public async Task<int> AddEventsSMSByStoreAsync(List<EventoSMSDto> dto)
        {
            var dynParams = new DynamicParameters();
            StringBuilder sql = new StringBuilder(1024);

            int i = 0;
            foreach (var item in dto)
            {
                dynParams.Add($"@idComunicacion{i}", item.IdComunicacion);
                dynParams.Add($"@fecha{i}", String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.Fecha));
                dynParams.Add($"@evento{i}", item.DEvento);
                dynParams.Add($"@idEvento{i}", item.IdEvento);
                dynParams.Add($"@idExterno{i}", item.IdExterno);
                dynParams.Add($"@message{i}", item.Message);
                dynParams.Add($"@telefonica{i}", item.Telefonica);

                sql.Append("EXEC dbo.AgregarEventoSMS ")
                    .Append($"@idComunicacion{i}")
                    .Append(",")
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
                    .Append($"@telefonica{i}")
                    .Append(";");
                i++;
            }
            await _context.Connection().ExecuteAsync(sql.ToString(), dynParams, null, commandTimeout: 300);
            return await _context.SaveChangesAsync();
        }

        public async Task<string> GetLastStateByIdComunicacionAsync(long idComunicacion)
        {
            var evento = await _context.EventoSMS
                .Select(s => s).Where(w=> w.IdComunicacion == idComunicacion).OrderByDescending(o => o.Fecha).ThenByDescending(o => o.IdEventoSMS).FirstOrDefaultAsync();
            if (evento == null) return "ENVIADA";
            switch (evento.DEvento.ToUpper().Trim())
            {
                case "PENDING":
                    return "ENVIADA";
                case "MT_DELIVERED":
                    return "RECIBIDO";
                case "REJECTED_BY_CARRIER":
                case "UNDEFINED":
                case "INVALID_DESTINATION_NUMBER":
                    return "FALLADO";
                default:
                    return "";
            }
        }

    }
}
