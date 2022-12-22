using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;

namespace Common.Services
{
    public class AvisoDeudaServices: IAvisoDeudaServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public AvisoDeudaServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AvisoDeudaDto> GetAsync(long id)
        {
            var aviso = await _unitOfWork.AvisoDeudaRepository().GetAsync(id);
            var response = _mapper.Map<AvisoDeudaDto>(aviso);
            return response;
        }
        public async Task<AvisoDeudaDto> GetAsync(string numeroComprobante)
        {
            var aviso = await _unitOfWork.AvisoDeudaRepository().GetAsync(numeroComprobante);
            var response = _mapper.Map<AvisoDeudaDto>(aviso);
            return response;
        }
    }
}
