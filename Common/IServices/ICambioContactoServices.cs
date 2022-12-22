using Common.Model.Dto;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface ICambioContactoServices
    {
        Task<long> AddContactChangeAsync(CambioContactoDto dto);
    }
}