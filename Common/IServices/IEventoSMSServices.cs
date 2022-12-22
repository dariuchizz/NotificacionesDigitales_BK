using Common.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IEventoSMSServices
    {
        Task<DateTime?> GetLastDateEventAsync();

        Task<long> AddEventsSMSByStoreAsync(List<EventoSMSDto> evento);

    }
}
