using Common.IServices;
using System.Threading.Tasks;

namespace Common.Services
{
    public class StoreServices : IStoreServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;

        public StoreServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<dynamic> GetAsync(string viewName, int top)
        {
            var response = await _unitOfWork.ViewRepository().GetDataViewAsync(viewName, top);
            return response;
        }

        public async Task<dynamic> GetStoreAsync(string storeName, int top, int timeOut = 240)
        {
            var response = await _unitOfWork.ViewRepository().GetDataStoreAsync(storeName, top, timeOut);
            return response;
        }
        public async Task<dynamic> GetStoreCampaniaAsync(string storeName, long idCampania, int top, int timeOut = 240)
        {
            var response = await _unitOfWork.ViewRepository().GetDataStoreAsync(storeName, idCampania, top, timeOut);
            return response;
        }

        public async Task<dynamic> GetStoreAsync(string storeName, int timeOut = 240)
        {
            var response = await _unitOfWork.ViewRepository().GetDataStoreAsync(storeName, timeOut);
            return response;
        }

        public async Task ExecuteAsync(string storeName, int timeOut = 240)
        {
            await _unitOfWork.ViewRepository().ExecuteAsync(storeName, timeOut);
        }
        public async Task ExecuteAsync(string storeName, long id, int timeOut = 240)
        {
            await _unitOfWork.ViewRepository().ExecuteAsync(storeName, id, timeOut);
        }
    }
}
