using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Services
{
    public class VencimientoFacturaServices : IVencimientoFacturaServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWorkNotificacion;
        private readonly IMapper _mapper;

        public VencimientoFacturaServices(IUnitOfWorkNotificacion unitOfWorkNotificacion, IMapper mapper)
        {
            _unitOfWorkNotificacion = unitOfWorkNotificacion;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SuministroDto>> GetAllAsync()
        {
            var cuentas = await _unitOfWorkNotificacion.CuentaRepository()
                            .GetAllAsync(cuenta => cuenta.Emails, cuenta2 => cuenta2.Facturas);
            return cuentas.Select(c => _mapper.Map<SuministroDto>(c));
        }
    }
}
