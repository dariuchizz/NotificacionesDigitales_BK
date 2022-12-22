using System.Threading.Tasks;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface ICsvCampaniaRepository: IGenericRepository<CsvCampania>
    {
        Task<CsvCampania> GetFirstRowAsync(long idCampania);
    }
}
