using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nexora.Management.API;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Infrastructure.Persistence.AppDbContext>
{
    public Infrastructure.Persistence.AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.EnableRetryOnFailure(3);
            options.CommandTimeout(30);
            options.MigrationsAssembly("Nexora.Management.API");
        });

        return new Infrastructure.Persistence.AppDbContext(optionsBuilder.Options);
    }
}
