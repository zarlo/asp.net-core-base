using Microsoft.EntityFrameworkCore;
using Zarlo.StarterWeb.Model;

namespace Zarlo.StarterWeb;

public class MainDbContext: DbContext
{

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

}
