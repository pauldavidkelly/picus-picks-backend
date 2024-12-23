using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;

namespace Picus.Api.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly PicusDbContext _context;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<PicusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PicusDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
