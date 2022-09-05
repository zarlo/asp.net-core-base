using Microsoft.EntityFrameworkCore;
using Zarlo.StarterWeb.Model;
using Microsoft.EntityFrameworkCore.Design;

namespace Zarlo.StarterWeb;

public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
        optionsBuilder.UseSqlite("Data Source=migrations.db");

        return new MainDbContext(optionsBuilder.Options);
    }
}
