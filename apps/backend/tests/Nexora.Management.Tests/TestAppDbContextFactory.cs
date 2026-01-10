using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Tests;

/// <summary>
/// Design-time factory for test DbContext that skips JSONB configurations
/// for InMemory database compatibility
/// </summary>
public class TestAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Use InMemory for tests
        optionsBuilder.UseInMemoryDatabase(
            Guid.NewGuid().ToString(),
            b => b.EnableNullChecks(false) // Disable null checks for InMemory flexibility
        );

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return new AppDbContext(optionsBuilder.Options);
    }
}
