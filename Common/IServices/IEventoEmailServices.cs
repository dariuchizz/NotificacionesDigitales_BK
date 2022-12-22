using Common.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IEventoEmailServices
    {
        Task<DateTime?> GetLastDateEventAsync();

        Task<long> AddEventsEmailsByStoreAsync(List<EventoEmailDto> evento);

    }
}
