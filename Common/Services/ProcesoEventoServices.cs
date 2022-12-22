using AutoMapper;
using Common.IServices;
using Common.Model.Enum;
using Common.Model.NotificacionesDigitales;
using System;
using System.Threading.Tasks;

namespace Common.Services
{
    public class ProcesoEventoServices : IProcesoEventoServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public ProcesoEventoServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> AddEventProcessByStoreAsync(ProcesoEventoDto dto)
        {
            return await _unitOfWork.ProcesoEventoRepository().AddEventProcessByStoreAsync(dto);
        }

        public async Task<long> AddProcesoEventoAsync(ProcesoEventoDto dto)
        {
            var procesoEvento = _mapper.Map<ProcesoEvento>(dto);
            await _unitOfWork.ProcesoEventoRepository().AddAsync(procesoEvento);
            await _unitOfWork.SaveChangeAsync();
            return procesoEvento.IdProcesoEvento;
        }

        public async Task UpdateProcesoEventoAsync(ProcesoEventoDto dto)
        {
            var procesoEvento = _mapper.Map<ProcesoEvento>(dto);
            await _unitOfWork.ProcesoEventoRepository().UpdateAsync(procesoEvento, pe => pe.IdProcesoEvento);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ProcesoEventoDto> GetAsync(long id)
        {
            var com = await _unitOfWork.ProcesoEventoRepository().FindByAsync(f => f.IdProcesoEvento == id);
            return _mapper.Map<ProcesoEventoDto>(com);
        }

        public async Task<ProcesoEventoDto> GetAsync(DateTime fechaAProcesar, TipoProcesoEvento tipoProcesoEvento, bool? Aviso)
        {
            var com = await _unitOfWork.ProcesoEventoRepository().FindByAsync(f => f.Tipo == (int)tipoProcesoEvento
                                                                                    && f.Aviso == Aviso
                                                                                    && f.Hora == fechaAProcesar.Hour
                                                                                    && f.Fecha.Date == fechaAProcesar.Date);
            return _mapper.Map<ProcesoEventoDto>(com);
        }
    }
}