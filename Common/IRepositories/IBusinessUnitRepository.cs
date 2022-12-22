using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Directory;

namespace Common.IRepositories
{
    public interface IBusinessUnitRepository: IGenericRepository<BusinessUnit>
    {
        Task<IEnumerable<BusinessUnit>> GetExcludingNullsAsync();
    }
}
