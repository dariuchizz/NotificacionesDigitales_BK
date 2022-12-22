using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IListaGrisServices
    {
        Task<ListaGrisDto> GetAsync(long idComunicacion);

        Task<long> AddByStoreAsync(UnsuscribeDto unsuscribeDto);
    }
}
