using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Common.Model.Directory
{
    public interface IDirectoryDbContext
    {
        DbSet<Enterprise> Enterprise { get; set; }

        DbSet<BusinessUnit> BusinessUnit { get; set; }

        DbSet<OperationCenter> OperationCenter { get; set; }

        DbSet<ManagmentCenter> ManagmentCenter { get; set; }
        DbSet<Agcpostlpf> Agcpostlpf { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        DatabaseFacade GetDatabase();

        DbSet<T> Set<T>() where T : class;

        EntityEntry<T> Entry<T>(T entity) where T : class;

        IDbConnection Connection();
    }
}
