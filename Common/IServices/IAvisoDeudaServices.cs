using System.Threading.Tasks;
using Common.Model.Dto;

namespace Common.IServices
{
    public interface IAvisoDeudaServices
    {
        Task<AvisoDeudaDto> GetAsync(long id);
        Task<AvisoDeudaDto> GetAsync(string numeroComprobante);
    }
}
