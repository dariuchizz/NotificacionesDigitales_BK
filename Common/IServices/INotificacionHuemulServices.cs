using Common.Model.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface INotificacionHuemulServices
    {

        Task<long> AddNotificacionesHuemulByStoreAsync(List<NotificacionHuemulDto> dto);

    }
}