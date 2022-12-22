using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class EmailRepository: GenericNotificacionRepository<Email>, IEmailRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public EmailRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Email> GetAsync(string correo)
        {
            var email = await _context.Email.FirstOrDefaultAsync(f => f.DEmail == correo);
            return email;
        }
    }
}
