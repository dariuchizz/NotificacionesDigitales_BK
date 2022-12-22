using Common.IServices;
using Common.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EventoSMSServices : IEventoSMSServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
     
        public EventoSMSServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DateTime?> GetLastDateEventAsync()
        {
            return await _unitOfWork.EventoEmailRepository().GetLastDateEventAsync();
        }

        public async Task<long> AddEventsSMSByStoreAsync(List<EventoSMSDto> dto)
        {
            return await _unitOfWork.EventoSMSRepository().AddEventsSMSByStoreAsync(dto);
        }
    }
}