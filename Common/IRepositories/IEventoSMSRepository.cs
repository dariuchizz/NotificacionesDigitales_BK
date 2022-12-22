using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface IEventoSMSRepository : IGenericRepository<EventoSMS>
    {

        Task<DateTime?> GetLastDateEventAsync();

        Task<int> AddEventsSMSByStoreAsync(List<EventoSMSDto> dto);
        Task<string> GetLastStateByIdComunicacionAsync(long idComunicacion);
    }
}
