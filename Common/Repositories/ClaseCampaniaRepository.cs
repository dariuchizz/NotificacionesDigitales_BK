using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class ClaseCampaniaRepository : GenericNotificacionRepository<ClaseCampania>, IClaseCampaniaRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public ClaseCampaniaRepository(INotificacionesDigitalesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
