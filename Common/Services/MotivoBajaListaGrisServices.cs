using System.Threading.Tasks;
using Common.IServices;
using AutoMapper;
using System.Collections.Generic;
using Common.Model.Services;
using Common.Model.Response;
using System.Linq;

namespace Common.Services
{
    public class MotivoBajaListaGrisServices : IMotivoBajaListaGrisServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public MotivoBajaListaGrisServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<IEnumerable<MotivoBajaListaGrisResponse>>> GetAllAsync()
        {
            var response = await _unitOfWork.MotivoBajaListaGrisRepository().GetAllAsync();
            var final = response.Select(s => new MotivoBajaListaGrisResponse
            {
                IdMotivoBajaListaGris = s.IdMotivoBajaListaGris,
                Descripcion = s.Descripcion,
                RequiereObservacion = s.RequiereObservacion
            });
            return ServiceResponseFactory.CreateOkResponse(final);
        }
    }
}
