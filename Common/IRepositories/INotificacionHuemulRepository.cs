using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface INotificacionHuemulRepository : IGenericRepository<NotificacionHuemul>
    {
        Task<int> AddNotificacionesHuemulByStoreAsync(List<NotificacionHuemulDto> dto);
    }
}