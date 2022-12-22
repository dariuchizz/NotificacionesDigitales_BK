using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Directory;

namespace Common.IRepositories
{
    public interface IAgcpostlpfRepository: IGenericRepository<Agcpostlpf>
    {
        Task<IEnumerable<Agcpostlpf>> GetByBusinessUnits(List<string> businessUnits);
        Task<List<string>> GetBusinessUnitsByZipCodeAsync(List<string> localidades);
    }
}
