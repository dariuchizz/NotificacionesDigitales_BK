using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.Services
{
    public class CanalServices: ICanalServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;

        public CanalServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<IEnumerable<CanalResponse>>> GetAllAsync()
        {
            var response = await _unitOfWork.CanalRepository().GetAllAsync();
            var final = response.Select(s => new CanalResponse
            {
                Id = s.IdCanal,
                Descripcion = s.Descripcion
            });
            return ServiceResponseFactory.CreateOkResponse(final);
        }
    }
}
