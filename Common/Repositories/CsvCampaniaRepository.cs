using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class CsvCampaniaRepository : GenericNotificacionRepository<CsvCampania>, ICsvCampaniaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public CsvCampaniaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CsvCampania> GetFirstRowAsync(long idCampania)
        {
            var response = await _context.CsvCampania.SingleAsync(s => s.IdCampania == idCampania && s.Secuencia == 1);
            return response;
        }
    }
}
