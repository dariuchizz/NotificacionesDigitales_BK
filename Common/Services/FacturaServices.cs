using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;

namespace Common.Services
{
    public class FacturaServices: IFacturaServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public FacturaServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FacturaDto> GetAsync(string numeroFactura)
        {
            var factura = await _unitOfWork.FacturaRepository().GetAsync(numeroFactura);
            var response = _mapper.Map<FacturaDto>(factura);
            return response;
        }

        public async Task<FacturaDto> GetAsync(long id)
        {
            var factura = await _unitOfWork.FacturaRepository().GetAsync(id);
            var response = _mapper.Map<FacturaDto>(factura);
            return response;
        }
    }
}
