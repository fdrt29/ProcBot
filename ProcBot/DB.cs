using Microsoft.EntityFrameworkCore;

namespace ProcBot;

internal sealed class UserContext : DbContext
{
    public UserContext()
    {
        Database.EnsureCreated();
    }

    // public bool SetState(){}

    public DbSet<Models.User> Users { get; private set; } // => SetUser(...);
    public DbSet<Models.Bill> Bills { get; private set; } // => SetUser(...);
    public DbSet<Models.Company> Companies { get; private set; } // => SetUser(...);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO mb use constructor with ContextOptions in Program
        optionsBuilder.UseSqlite($"Data Source={Program.Configuration.DatabaseSource}");
        // optionsBuilder.LogTo();
    }
}