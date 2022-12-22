using Common.IServices;
using Common.Model.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public class NotificacionHuemulServices : INotificacionHuemulServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;

        public NotificacionHuemulServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> AddNotificacionesHuemulByStoreAsync(List<NotificacionHuemulDto> dto)
        {
            return await _unitOfWork.NotificacionHuemulRepository().AddNotificacionesHuemulByStoreAsync(dto);
        }

    }
}