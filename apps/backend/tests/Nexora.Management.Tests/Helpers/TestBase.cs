using Microsoft.EntityFrameworkCore;
using Nexora.Management.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.Tests.Helpers;

public abstract class TestBase : IAsyncDisposable
{
    protected readonly AppDbContext DbContext;
    protected readonly CancellationToken CancellationToken = CancellationToken.None;

    protected TestBase()
    {
        // Use InMemory for tests - JSONB columns automatically ignored by AppDbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        DbContext = new AppDbContext(options);
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync(CancellationToken);
    }
}
