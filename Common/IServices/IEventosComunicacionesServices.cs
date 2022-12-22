using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model.Dto;
using Common.Model.Services;

namespace Common.IServices
{
    public interface IEventosComunicacionesServices
    {
        Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> GetEventosByIdComunicacionAsync(
            long idComunicacion);
    }
}
