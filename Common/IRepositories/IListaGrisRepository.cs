using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface IListaGrisRepository : IGenericRepository<ListaGris>
    {
        Task<int> AddByStoreAsync(UnsuscribeDto unsuscribeDto);
    }
}
