using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.Directory;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class BusinessUnitRepository: GenericDirectoryRepository<BusinessUnit>, IBusinessUnitRepository
    {
        private readonly IDirectoryDbContext _context;
        public List<string> excludedBusinessUnitList = new List<string>
        {
            "GES",
            "NEC",
            "TSA",
        };
        public BusinessUnitRepository(IDirectoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BusinessUnit>> GetExcludingNullsAsync()
        {
            var response = await _context.BusinessUnit.Where(w => w.Code != null && excludedBusinessUnitList.All(a => a != w.Code)).ToArrayAsync();
            return response;
        }
    }
}
