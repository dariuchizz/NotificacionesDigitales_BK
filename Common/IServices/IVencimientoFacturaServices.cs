using Common.Model.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IVencimientoFacturaServices
    {
        Task<IEnumerable<SuministroDto>> GetAllAsync();
    }
}
