using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Services;

namespace Common.Services
{
    public class EventosComunicacionesServices : IEventosComunicacionesServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public EventosComunicacionesServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<IEnumerable<EventoComunicacionDto>>> GetEventosByIdComunicacionAsync(
            long idComunicacion)
        {
            var response = await _unitOfWork.EventosComunicacionesRepository()
                .GetEventosByIdComunicacionAsync(idComunicacion);
            return ServiceResponseFactory.CreateOkResponse(response);
        }
    }
}
