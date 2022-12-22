using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;

namespace Common.IRepositories
{
    public interface IStoreRepository
    {
        Task<dynamic> GetDataViewAsync(string viewName, int top);

        Task<dynamic> GetDataStoreAsync(string storeName, int top, int timeOut);
        Task<dynamic> GetDataStoreAsync(string storeName, long idCampania, int top, int timeOut);
        Task<dynamic> GetDataStoreAsync(string storeName, int timeOut);

        Task<dynamic> ExecuteAsync(string storeName, int timeOut = 120);
        Task<dynamic> ExecuteAsync(string storeName, long id, int timeOut = 120);

        Task<IEnumerable<ConsultarCampaniaDto>> GetDatosCampaniaAsync(string storeName, long idCampania,
            int timeOut = 120);
    }
}
