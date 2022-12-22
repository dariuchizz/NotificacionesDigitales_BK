using Common.IRepositories;

namespace Common.IServices
{
    public interface IUnitOfWorkDirectory
    {
        IAgcpostlpfRepository AgcpostlpfRepository();
        IBusinessUnitRepository BusinessUnitRepository();
    }
}
