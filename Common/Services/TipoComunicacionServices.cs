using System.Collections.Generic;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using System.Threading.Tasks;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.Services
{
    public class TipoComunicacionServices : ITipoComunicacionServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public TipoComunicacionServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TipoComunicacionDto> GetAsync(int id)
        {
            var tipoComunicacion = await _unitOfWork.TipoComunicacionRepository().FindByAsync(comunicacion => comunicacion.IdTipoComunicacion == id);
            return _mapper.Map<TipoComunicacionDto>(tipoComunicacion);
        }

        public async Task<ServiceResponse<IEnumerable<TipoComunicacionesResponse>>> GetAllAsync()
        {
            var response = await _unitOfWork.TipoComunicacionRepository().GetAllWithEnviosAsync();
            return ServiceResponseFactory.CreateOkResponse(response);
        }
    }
}
