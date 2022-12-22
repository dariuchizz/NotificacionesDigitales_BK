using Common.IRepositories;
using Common.IServices;
using Common.Model.Directory;
using Common.Repositories;

namespace Common.Services
{
    public class UnitOfWorkDirectory : IUnitOfWorkDirectory
    {
        private IDirectoryDbContext _context;

        public UnitOfWorkDirectory(IDirectoryDbContext context)
        {
            _context = context;
        }

        public IAgcpostlpfRepository AgcpostlpfRepository()
        {
            var repo = new AgcpostlpfRespository(_context);
            return repo;
        }

        public IBusinessUnitRepository BusinessUnitRepository()
        {
            var repo = new BusinessUnitRepository(_context);
            return repo;
        }
    }
}
