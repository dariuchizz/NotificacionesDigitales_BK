using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Services;

namespace Common.Services
{
    public class MotivoBajaServices : IMotivoBajaServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWorkNotificacion;

        public MotivoBajaServices(IUnitOfWorkNotificacion unitOfWorkNotificacion)
        {
            _unitOfWorkNotificacion = unitOfWorkNotificacion;
        }

        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboMotivoBajaByTipoMotivoAsync(string tipoMotivo)
        {
            var response = await _unitOfWorkNotificacion.MotivoBajaRepository()
                .SearchByAsync(s => s.TipoMotivo == tipoMotivo.ToUpper().Trim());
            var combo = response.Select(s => new ComboLongDto
            {
                Id = s.IdMotivoBaja,
                Descripcion = s.Descripcion.Trim()
            });
            return ServiceResponseFactory.CreateOkResponse(combo);
        }
    }
}
