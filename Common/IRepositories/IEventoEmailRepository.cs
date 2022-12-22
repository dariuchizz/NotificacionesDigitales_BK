using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface IEventoEmailRepository : IGenericRepository<EventoEmail>
    {
        Task<DateTime?> GetLastDateEventAsync();

        Task<int> AddEventsEmailsByStoreAsync(List<EventoEmailDto> dto);

        Task<string> GetLastEventByIdComunicacionAsync(long idComunicacion);

    }
}
