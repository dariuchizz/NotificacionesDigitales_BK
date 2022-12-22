using Common.IServices;
using Common.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EventoEmailServices : IEventoEmailServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;

        public EventoEmailServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DateTime?> GetLastDateEventAsync()
        {
            return await _unitOfWork.EventoEmailRepository().GetLastDateEventAsync();
        }

        public async Task<long> AddEventsEmailsByStoreAsync(List<EventoEmailDto> dto)
        {
            return await _unitOfWork.EventoEmailRepository().AddEventsEmailsByStoreAsync(dto);
        }

    }
}