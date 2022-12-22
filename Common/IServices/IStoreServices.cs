using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IStoreServices
    {
        Task<dynamic> GetAsync(string viewName, int top);

        Task<dynamic> GetStoreAsync(string storeName, int top, int timeOut = 120);
        Task<dynamic> GetStoreCampaniaAsync(string storeName, long idCampania, int top, int timeOut = 120);

        Task<dynamic> GetStoreAsync(string storeName, int timeOut = 120);

        Task ExecuteAsync(string storeName, int timeOut = 120);
        Task ExecuteAsync(string storeName, long id, int timeOut = 120);
    }
}
