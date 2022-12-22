using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Common.Model.Directory
{
    public class DirectoryDbContext : DbContext, IDirectoryDbContext
    {
        public DirectoryDbContext(DbContextOptions<DirectoryDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Enterprise> Enterprise { get; set; }

        public DbSet<BusinessUnit> BusinessUnit { get; set; }

        public DbSet<OperationCenter> OperationCenter { get; set; }

        public DbSet<ManagmentCenter> ManagmentCenter { get; set; }
        public DbSet<Agcpostlpf> Agcpostlpf { get; set; }
        public IDbConnection Connection()
        {
            return Database.GetDbConnection();
        }

        public DatabaseFacade GetDatabase()
        {
            return Database;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enterprise>()
                   .HasMany(c => c.BusinessUnits)
                   .WithOne(e => e.Enterprise);

            modelBuilder.Entity<BusinessUnit>()
                  .HasMany(c => c.OperationCenters)
                  .WithOne(e => e.BusinessUnit);

            modelBuilder.Entity<OperationCenter>()
                .HasMany(c => c.ManagmentCenters)
                .WithOne(e => e.OperationCenter);

            modelBuilder.Entity<Agcpostlpf>()
                .HasKey(k => new {k.Agcpcodigo, k.Agcpdigito});
        }
    }
}
