using System.Threading.Tasks;
using Common.Model.Dto;

namespace Common.IServices
{
    public interface IFacturaServices
    {
        Task<FacturaDto> GetAsync(string numeroFactura);
        Task<FacturaDto> GetAsync(long id);
    }
}
