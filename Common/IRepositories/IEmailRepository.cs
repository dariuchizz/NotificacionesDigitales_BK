using System.Threading.Tasks;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface IEmailRepository: IGenericRepository<Email>
    {
        Task<Email> GetAsync(string email);
    }
}
