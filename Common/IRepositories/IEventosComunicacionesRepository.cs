using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface IEventosComunicacionesRepository: IGenericRepository<EventosComunicaciones>
    {
        Task<IEnumerable<EventoComunicacionDto>> GetEventosByIdRelacionadoAsync(ComunicacionType comunicacion,
            long idRelacionado, int maxTake);

        Task<IEnumerable<EventoComunicacionDto>> GetEventosByIdComunicacionAsync(long idComunicacion);
    }
}
