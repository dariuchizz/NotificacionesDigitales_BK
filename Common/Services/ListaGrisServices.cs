using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using AutoMapper;
using Common.Model.NotificacionesDigitales;
using System;

namespace Common.Services
{
    public class ListaGrisServices : IListaGrisServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public ListaGrisServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ListaGrisDto> GetAsync(long idComunicacion)
        {
            try
            {
                var list = await _unitOfWork.ListaGrisRepository().FindByAsync(f => f.IdComunicacion == idComunicacion);
                var listaGris = _mapper.Map<ListaGrisDto>(list);
                return listaGris;
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<long> AddByStoreAsync(UnsuscribeDto unsuscribeDto)
        {
            return await _unitOfWork.ListaGrisRepository().AddByStoreAsync(unsuscribeDto);
        }
    }
}
