using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.IRepositories;
using Common.Model.Directory;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class AgcpostlpfRespository : GenericDirectoryRepository<Agcpostlpf>, IAgcpostlpfRepository
    {
        private readonly IDirectoryDbContext _context;

        public AgcpostlpfRespository(IDirectoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Agcpostlpf>> GetByBusinessUnits(List<string> businessUnits)
        {
            var empresas = new List<string> { "CGP", "CGS" };
            var ctx = await _context.Agcpostlpf
                .Where(w => empresas.Contains(w.Agcpempres) && w.Agcpunineg != "" &&
                            (businessUnits.Contains(w.Agcpunineg) || businessUnits.Count == 0))
                .ToArrayAsync();
            return ctx;
        }

        public async Task<List<string>> GetBusinessUnitsByZipCodeAsync(List<string> localidades)
        {
            var businessUnits = await _context.Agcpostlpf
                .Where(w=> localidades.Contains(w.Agcpcodigo.Trim()+"-"+w.Agcpdigito.Trim()))
                .Select(s=> s.Agcpunineg).Distinct()
                .ToListAsync();
            return businessUnits;
        }
    }
}
