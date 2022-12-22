using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;


namespace Common.Services
{
    public class CambioContactoServices : ICambioContactoServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public CambioContactoServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> AddContactChangeAsync(CambioContactoDto dto)
        {
            var cambioContacto = _mapper.Map<CambioContacto>(dto);
            await _unitOfWork.CambioContactoRepository().AddAsync(cambioContacto);
            await _unitOfWork.SaveChangeAsync();
            return cambioContacto.IdCambioContacto;
        }
    }
}